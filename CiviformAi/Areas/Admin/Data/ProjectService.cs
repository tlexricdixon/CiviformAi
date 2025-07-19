using CiviformAi.Models;
using Contracts;
using Entities;
using System.Text.Json;
using Microsoft.AspNetCore.Hosting;

namespace CiviformAi.Areas.Admin.Data
{
    public class ProjectService
    {
        private readonly IWebHostEnvironment _env;
        private readonly IAccessFileService _accessFileService;
        public ProjectService(IWebHostEnvironment env, IAccessFileService accessFileService)
        {
            _env = env;
            _accessFileService = accessFileService;
        }

        public string GetProjectPath(string name) =>
            Path.Combine(_env.ContentRootPath, "Projects", name);

        public async Task SaveConfigAndFileAsync(ProjectSettings settings, IFormFile accessFile)
        {
            var dir = GetProjectPath(settings.ProjectName);
            Directory.CreateDirectory(dir);
            await File.WriteAllTextAsync(
                Path.Combine(dir, "config.json"),
                JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true })
            );

            if (accessFile?.Length > 0)
            {
                _accessFileService.SaveAccessFileAsync(
                    accessFile,
                    dir,
                    Path.Combine(dir, "config.json")
                ).GetAwaiter().GetResult();
            }
        }
    }
}

