using Contracts;
using System.Data;
using System.Data.OleDb;

namespace Repository;
public class AccessImportService : IAccessImportService
{
    public async Task Import(string accessFilePath)
    {
        var connectionString = $@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={accessFilePath};Persist Security Info=False;";

        using var connection = new OleDbConnection(connectionString);
        connection.Open();

        var tables = connection.GetSchema("Tables")
            .AsEnumerable()
            .Where(r => r["TABLE_TYPE"].ToString() == "TABLE")
            .Select(r => r["TABLE_NAME"].ToString())
            .ToList();

        foreach (var table in tables)
        {
            var cmd = new OleDbCommand($"SELECT * FROM [{table}]", connection);
            using var reader = cmd.ExecuteReader();

            var dt = new DataTable();
            dt.Load(reader);

            // await _yourDbService.SaveTableAsync(table, dt); // dynamically save
        }
    }
}
