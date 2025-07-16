using Contracts;
using System.Data;
using System.Data.OleDb;

namespace Repository;
public class AccessDbService : IAccessDbService
{
    public List<string> GetTableNames(string accessFilePath)
    {
        var tables = new List<string>();
        var connectionString = $"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={accessFilePath};Persist Security Info=False;";

        using var connection = new OleDbConnection(connectionString);
        connection.Open();

        DataTable schema = connection.GetSchema("Tables");
        foreach (DataRow row in schema.Rows)
        {
            string tableType = row["TABLE_TYPE"].ToString();
            if (tableType == "TABLE")
            {
                tables.Add(row["TABLE_NAME"].ToString());
            }
        }

        return tables;
    }

    public List<(string ColumnName, string DataType)> GetTableSchema(string accessFilePath, string tableName)
    {
        var columns = new List<(string, string)>();
        var connectionString = $"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={accessFilePath};Persist Security Info=False;";

        using var connection = new OleDbConnection(connectionString);
        connection.Open();

        using var cmd = new OleDbCommand($"SELECT * FROM [{tableName}] WHERE 1=0", connection); // no data, just schema
        using var reader = cmd.ExecuteReader();

        var schemaTable = reader.GetSchemaTable();
        foreach (DataRow row in schemaTable.Rows)
        {
            string columnName = row["ColumnName"].ToString();
            string dataType = row["DataType"].ToString();
            columns.Add((columnName, dataType));
        }

        return columns;
    }

    public async Task SaveTableAsync(string tableName, DataTable tableData)
    {
        // OPTIONAL: Standardize names, strip spaces
        tableName = tableName.Replace(" ", "_");

        // If you want to store generically (No EF models), use ADO.NET:
        // CREATE TABLE if not exists
        // INSERT rows one by one or bulk copy

        //// If you want to map to EF models, do conditional mappings:
        //if (tableName == "Products")
        //    SaveToProductsModel(tableData);
        //else if (tableName == "Customers")
        //    SaveToCustomersModel(tableData);
        //else
        //    SaveToGenericJsonStore(tableData);
    }

}
