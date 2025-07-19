using Entities;
using Microsoft.AspNetCore.Http;

namespace Contracts
{
    public interface IAccessSchemaService
    {
        Task<List<TableSchema>> CreateAccessSchemaAsync(List<IFormFile> files, string basePath);
        Task SaveAccessSchemaAsync(List<TableSchema> schema, string basePath);
        List<TableSchema> LoadAccessSchema(string basePath);

    }
}
