using Contracts;
using System.Data;
using System.Data.OleDb;

namespace Repository;
public class AccessDbService : IAccessDbService
{
    public List<string> GetTableNames(string filePath)
    {
        var connStr = $@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={filePath};Persist Security Info=False;";
        var tableNames = new List<string>();

        using var conn = new OleDbConnection(connStr);
        conn.Open();
        // Fetch user tables
        var dt = conn.GetSchema("Tables");
        foreach (DataRow row in dt.Rows)
        {
            if (row["TABLE_TYPE"].ToString() == "TABLE")
                tableNames.Add(row["TABLE_NAME"].ToString());
        }
        return tableNames;
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
