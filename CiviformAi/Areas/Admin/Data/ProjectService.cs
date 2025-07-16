using CiviformAi.Areas.Admin.Models;
using System.Text.Json;

namespace CiviformAi.Areas.Admin.Data
{
    internal class ProjectService(IWebHostEnvironment env)
    {
        private readonly string path = env.ContentRootPath;
        public void Save(ProjectSettings config)
        {
            var p = Path.Combine(path, "Projects", config.ProjectName);
            Directory.CreateDirectory(p);
            var configPath = Path.Combine(p, "config.json");

            var options = new JsonSerializerOptions { WriteIndented = true };
            var json = JsonSerializer.Serialize(config, options);
            File.WriteAllText(configPath, json);
        }
    }
}
