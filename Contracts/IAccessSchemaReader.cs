using Microsoft.AspNetCore.Http;

namespace Contracts
{
    public interface IAccessSchemaReader<T>
    {
        Task<List<T>> GetAccessSchemaAsync(List<IFormFile> files, string path);
    }
}
