using Microsoft.AspNetCore.Http;
using Entities;
using Microsoft.AspNetCore.Hosting;

namespace Contracts
{
    public interface IAccessFileService
    {
        Task SaveAccessFileAsync(IFormFile accessFile, string project, string path);
    }

}
