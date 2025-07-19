using CiviformAi.Models;
using Contracts;
using Entities;
using System.Text.Json;

namespace CiviformAi.Areas.Admin.Data
{
    public class ProjectStore
    {
        private readonly string _basePath;
        private readonly string _projectPath;
        private readonly ProjectService _projectService;

        public ProjectStore(IWebHostEnvironment env, IAccessFileService accessFileService)
        {
            _projectService = new ProjectService(env,accessFileService);
            _projectPath = env.ContentRootPath;
            _basePath = Path.Combine(env.ContentRootPath, "Projects");
            Directory.CreateDirectory(_basePath);
        }

        public List<string> ListProjects() =>
            [.. Directory.GetDirectories(_basePath).Select(Path.GetFileName)];

        internal void Save(ProjectSettings settings)
        {
            var path = _projectService.GetProjectPath(settings.ProjectName);
            Directory.CreateDirectory(path);

            var json = JsonSerializer.Serialize(settings, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            File.WriteAllText(Path.Combine(path, "config.json"), json);
        }

        internal ProjectSettings Load(string projectName)
        {
            var path = Path.Combine(_projectPath, "Projects", projectName, "config.json");
            return File.Exists(path)
                ? JsonSerializer.Deserialize<ProjectSettings>(File.ReadAllText(path))
                : null;
        }
    }
}
