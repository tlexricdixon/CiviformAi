using Entities;
using Microsoft.AspNetCore.Http;

namespace Contracts
{
    public interface IAccessSchemaService
    {
        Task SaveAccessSchemaAsync(List<TableSchema> schema, string basePath);
        List<TableSchema> LoadAccessSchema(string basePath);

    }
}
