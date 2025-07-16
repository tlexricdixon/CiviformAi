using Microsoft.AspNetCore.Http;

namespace Contracts;

public interface IAccessImportService<T>
{
    Task Import(T table);
    string GenerateSQL(T schemaMap);
    Task<List<Dictionary<string, object>>> MigrateTableDataAsync(string accessFilePath, string tableName);
}
