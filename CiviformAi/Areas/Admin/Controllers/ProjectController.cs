using CiviformAi.Areas.Admin.Data;
using Contracts;
using Entities;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
namespace CiviformAi.Areas.Admin.Controllers;

[Area("Admin")]
public class ProjectController(IWebHostEnvironment env, IAccessFileService accessFileService) : Controller
{
    private readonly ProjectStore _projectStore = new(env, accessFileService );
    private readonly ProjectService _projectService = new(env, accessFileService);
    [HttpGet]
    public IActionResult Index()
    {
        var projects = _projectStore.ListProjects();
        return View(projects);
    }

    public IActionResult StartProject() => View(new ProjectConfigViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> StartProject(ProjectConfigViewModel vm, IFormFile AccessFile)
    {
        if (!ModelState.IsValid) return View(vm);

        var settings = ProjectSettings.FromVm(vm);
        settings.AccessFileName = vm.AccessFile?.FileName ?? string.Empty;

        var dir = Path.Combine(env.ContentRootPath, "Projects", vm.ProjectName);
        Directory.CreateDirectory(dir);

        if (AccessFile != null)
        {
            await _projectService.SaveConfigAndFileAsync(settings, AccessFile);
        }
        else
        {
            ModelState.AddModelError(string.Empty, "Access file is required.");
            return View(vm);
        }

        _projectStore.Save(settings);

        return RedirectToAction("Index", "Dashboard", new { area = "Project", project = settings.ProjectName });
    }


    public IActionResult Configure(string projectName)
    {
        var config = _projectStore.Load(projectName);
        return config == null ? RedirectToAction("StartProject") : View("StartProject", config);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Configure(ProjectConfigViewModel vm)
    {
        if (!ModelState.IsValid) return View("StartProject", vm);

        var settings = ProjectSettings.FromVm(vm);
        await _projectService.SaveConfigAndFileAsync(settings, vm.AccessFile);

        return RedirectToAction("Dashboard", new { project = settings.ProjectName });
    }

    public async Task<bool> TestSqlConnectionAsync(string server, string dbName, string user, string password, bool integrated)
    {
        var builder = new SqlConnectionStringBuilder
        {
            DataSource = server,
            InitialCatalog = dbName,
            IntegratedSecurity = integrated
        };

        if (!integrated)
        {
            builder.UserID = user;
            builder.Password = password;
        }

        try
        {
            using var conn = new SqlConnection(builder.ConnectionString);
            await conn.OpenAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }
}
