using Contracts;
using Entities;
using Microsoft.AspNetCore.Http;
using System.Data;
using System.Data.OleDb;

namespace Repository;
public class AccessSchemaReader : IAccessSchemaReader<TableSchema>
{
    public async Task<List<TableSchema>> GetAccessSchemaAsync(List<IFormFile> files, string basePath)
    {
        var result = new List<TableSchema>();
        var uploadDir = Path.Combine(basePath, "TempUploads");

        Directory.CreateDirectory(uploadDir); // Ensure the directory exists

        foreach (var file in files)
        {
            var extension = Path.GetExtension(file.FileName);
            if (extension != ".accdb" && extension != ".mdb")
                continue;

            var savePath = Path.Combine(uploadDir, Path.GetRandomFileName() + extension);

            await using (var stream = file.OpenReadStream())
            await using (var fs = new FileStream(savePath, FileMode.Create))
            {
                await stream.CopyToAsync(fs);
            }

            await Task.Delay(50); // Small delay to ensure file is released

            var reader = new AccessSchemaReader();
            var tables = reader.GetTableNames(savePath);

            foreach (var table in tables)
            {
                var columnList = reader.GetTableSchema(savePath, table);
                var columnSchemas = columnList.Select(col => new ColumnSchema
                {
                    ColumnName = col.ColumnName,
                    DataType = col.DataType?.ToString() ?? "UNKNOWN",
                    SqlType = col.SqlType
                }).ToList();

                result.Add(new TableSchema
                {
                    TableName = table,
                    Columns = columnSchemas,
                    FileName = Path.GetFileName(savePath) // Preserve for later data migration
                });
            }
        }

        return result;
    }

    private string ConvertJetCodeToSqlType(int jetCode)
    {
        return jetCode switch
        {
            1 => "BIT",                   // Yes/No
            2 => "SMALLINT",              // Byte
            3 => "SMALLINT",             // Integer
            4 => "INT",                  // Long
            5 => "MONEY",                // Currency
            6 => "REAL",                 // Single
            7 => "DATETIME",                // Double
            8 => "DATETIME",             // Date/Time
            9 => "INT IDENTITY(1,1)",    // AutoNumber (legacy)
            10 => "VARCHAR(255)",         // Text
            11 => "VARBINARY(MAX)",       // OLE Object
            12 => "VARCHAR(MAX)",         // Memo
            13 => "UNIQUEIDENTIFIER",     // Replication ID (legacy)
            14 => "TEXT",                 // Complex (Not well supported)
            15 => "VARBINARY(MAX)",       // Attachment
            16 => "UNIQUEIDENTIFIER",     // GUID (reserved)
            17 => "DECIMAL(18,4)",     // GUID (standard)
            18 => "BIGINT",               // BigInt (ACE-only)
            128 => "VARBINARY(255)",       // Binary
            129 => "CHAR(255)",            // Char
            130 => "NVARCHAR(MAX)",        // Unicode Text
            131 => "DECIMAL(18,4)",        // Decimal
            _ => "UNKNOWN"
        };
    }

    public List<string> GetTableNames(string filePath)
    {
        var tableNames = new List<string>();
        using var connection = new OleDbConnection(GetConnectionString(filePath));
        connection.Open();

        // Get tables, exclude system/internal tables
        var schema = connection.GetSchema("Tables");
        foreach (DataRow row in schema.Rows)
        {
            var tableType = row["TABLE_TYPE"].ToString();
            var tableName = row["TABLE_NAME"].ToString();
            if (tableType == "TABLE" && !tableName.StartsWith("MSys"))
                tableNames.Add(tableName);
        }

        return tableNames;
    }

    public List<ColumnSchema> GetTableSchema(string filePath, string tableName)
    {
        var columns = new List<ColumnSchema>();

        using var connection = new OleDbConnection(GetConnectionString(filePath));
        connection.Open();

        var schema = connection.GetSchema("Columns", new[] { null, null, tableName, null });

        foreach (DataRow row in schema.Rows)
        {
            // Convert DATA_TYPE to int and then map to SQL type
            var dataTypeObj = row["DATA_TYPE"];
            int jetCode = dataTypeObj != null ? Convert.ToInt32(dataTypeObj) : -1;

            columns.Add(new ColumnSchema
            {
                ColumnName = row["COLUMN_NAME"]?.ToString() ?? "UNKNOWN",
                DataType = jetCode.ToString(),
                SqlType = ConvertJetCodeToSqlType(jetCode)
            });
        }

        return columns;
    }


    private string GetConnectionString(string filePath)
    {
        return $@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={filePath};Persist Security Info=False;";
    }
}

