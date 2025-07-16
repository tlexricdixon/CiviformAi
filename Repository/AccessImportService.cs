using Contracts;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Data.OleDb;
using System.Text;

namespace Repository;
public class AccessImportService : IAccessImportService<TableSchema>
{
    private readonly string connectionString =
    "Data Source=LAPTOP-57G6RU3I\\TLEXDEVSQL;" +
    "Initial Catalog=Civiformai;" +
    "Integrated Security=True;" +
    "TrustServerCertificate=True;";


    public string GenerateSQL(TableSchema schemaMap)
    {
        var sqlBuilder = new StringBuilder();

        // Add DROP IF EXISTS
        sqlBuilder.AppendLine($"IF OBJECT_ID(N'[dbo].[{schemaMap.TableName}]', N'U') IS NOT NULL");
        sqlBuilder.AppendLine($"    DROP TABLE [dbo].[{schemaMap.TableName}];");
        sqlBuilder.AppendLine();

        // Add CREATE TABLE
        sqlBuilder.AppendLine($"CREATE TABLE [dbo].[{schemaMap.TableName}] (");

        foreach (var col in schemaMap.Columns)
        {
            sqlBuilder.AppendLine($"    [{col.ColumnName}] {col.SqlType},");
        }

        if (schemaMap.Columns.Count > 0)
            sqlBuilder.Length -= 3; // Remove the trailing comma and newline

        sqlBuilder.AppendLine("\n);");
        return sqlBuilder.ToString();
    }


    public async Task Import(TableSchema table)
    {
        string sql = GenerateSQL(table);
        using SqlConnection connection = new();
        using var conn = new SqlConnection(connectionString);
        await conn.OpenAsync(); // <<< make sure this completes before proceeding

        using var cmd = new SqlCommand(sql, conn);
        await cmd.ExecuteNonQueryAsync();
    }
    public async Task<List<Dictionary<string, object>>> MigrateTableDataAsync(string accessFilePath, string tableName)
    {
        var accessConnStr = $"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={accessFilePath};Persist Security Info=False;";
        

        var insertedRows = new List<Dictionary<string, object>>();

        using var accessConn = new OleDbConnection(accessConnStr);
        await accessConn.OpenAsync();

        using var readerCmd = new OleDbCommand($"SELECT * FROM [{tableName}]", accessConn);
        using var reader = await readerCmd.ExecuteReaderAsync();

        using var sqlConn = new SqlConnection(connectionString);
        await sqlConn.OpenAsync();

        var schemaTable = reader.GetSchemaTable();
        var columnNames = schemaTable.Rows
            .Cast<DataRow>()
            .Select(row => row["ColumnName"].ToString())
            .ToList();

        var columnTypes = schemaTable.Rows
            .Cast<DataRow>()
            .ToDictionary(row => row["ColumnName"].ToString(), row => Convert.ToInt32(row["ProviderType"]));

        while (await reader.ReadAsync())
        {
            var rowDict = new Dictionary<string, object>();

            var insertSql = $"INSERT INTO [{tableName}] ({string.Join(",", columnNames)}) VALUES ({string.Join(",", columnNames.Select(c => "@" + c))})";
            using var insertCmd = new SqlCommand(insertSql, sqlConn);

            foreach (var col in columnNames)
            {
                var value = reader[col];
                var jetType = columnTypes[col];

                // Handle known Access to SQL quirks
                if (jetType == 17 && value is byte b && b == 0) // Jet 17 → UNIQUEIDENTIFIER
                    value = DBNull.Value;

                insertCmd.Parameters.AddWithValue("@" + col, value ?? DBNull.Value);
                rowDict[col] = value ?? DBNull.Value;
            }

            await insertCmd.ExecuteNonQueryAsync();
            insertedRows.Add(rowDict);
        }
        return insertedRows;
    }
}
