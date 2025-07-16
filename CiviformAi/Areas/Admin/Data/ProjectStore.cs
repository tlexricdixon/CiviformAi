using CiviformAi.Areas.Admin.Models;
using Newtonsoft.Json;

namespace CiviformAi.Areas.Admin.Data
{
    public class ProjectStore
    {
        private readonly string _basePath;

        public ProjectStore(IWebHostEnvironment env)
        {
            _basePath = Path.Combine(env.ContentRootPath, "App_Data", "Projects");
            Directory.CreateDirectory(_basePath);
        }

        public void Save(ProjectConfigViewModel config)
        {
            var dir = Path.Combine(_basePath, config.ProjectName);
            Directory.CreateDirectory(dir);
            var filePath = Path.Combine(dir, "config.json");
            var json = JsonConvert.SerializeObject(config, Formatting.Indented);
            File.WriteAllText(filePath, json);
        }

        public ProjectConfigViewModel? Load(string projectName)
        {
            var filePath = Path.Combine(_basePath, projectName, "config.json");
            if (!File.Exists(filePath)) return null;
            var json = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<ProjectConfigViewModel>(json);
        }

        public List<string> ListProjects() =>
            Directory.GetDirectories(_basePath).Select(Path.GetFileName).ToList();
    }
}
