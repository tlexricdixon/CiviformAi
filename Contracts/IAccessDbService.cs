using System.Data;

namespace Contracts;

public interface IAccessDbService
{

    /// <summary>
    /// Saves a table from an Access database to the local database.
    /// </summary>
    /// <param name="tableName">The name of the table to save.</param>
    /// <param name="tableData">The data of the table as a DataTable.</param>
    Task SaveTableAsync(string tableName, DataTable tableData);
}
