using CiviformAi.Areas.Admin.Data;
using CiviformAi.Areas.Admin.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Text.Json;
namespace CiviformAi.Areas.Admin.Controllers;

[Area("Admin")]
public class ProjectController(IWebHostEnvironment env) : Controller
{
    private readonly IWebHostEnvironment _env = env;
    private readonly ProjectStore _projectStore = new(env);
    private readonly ProjectService _projectService = new(env);
    private readonly string path = env.ContentRootPath;
    [HttpGet]
    public IActionResult Index()
    {
        var projects = _projectStore.ListProjects();
        return View(projects);
    }

    public IActionResult StartProject() => View(new ProjectConfigViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> StartProject(ProjectConfigViewModel config, IFormFile File)
    {
        if (!ModelState.IsValid)
            return View(config);

        var projectPath = _projectService.GetProjectPath(config.ProjectName);
        await _projectService.SaveConfigAsync(projectPath, config);
        await _projectService.SaveAccessFileAsync(projectPath, File);

        _projectStore.Save(config);

        return RedirectToAction("Index", "Dashboard", new { area = "Project", project = config.ProjectName });
    }

    public IActionResult Configure(string projectName)
    {
        var config = _projectStore.Load(projectName);
        return config == null ? RedirectToAction("StartProject") : View("StartProject", config);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Configure(ProjectConfigViewModel model)
    {
        if (!ModelState.IsValid)
            return View("StartProject", model);

        var projectPath = _projectService.GetProjectPath(model.ProjectName);
        await _projectService.SaveConfigAsync(projectPath, model);
        await _projectService.SaveAccessFileAsync(projectPath, model.AccessFile);

        return RedirectToAction("Dashboard", "Project", new { projectName = model.ProjectName });
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

    //[HttpPost]
    //public async Task<IActionResult> TestDbConnection(ProjectConfigurationViewModel model)
    //{
    //    var success = await _dbTester.TestSqlConnectionAsync(
    //        model.DbServer,
    //        model.DatabaseName,
    //        model.DbUser,
    //        model.DbPassword,
    //        model.UseIntegratedSecurity
    //    );

    //    TempData["ConnectionResult"] = success ? "✅ Connection successful!" : "❌ Failed to connect.";
    //    TempData["ConnectionSuccess"] = success;

    //    return RedirectToAction("Configure", new { projectName = model.ProjectName });
    //}

    //[HttpPost]
    //public IActionResult SaveDbConfig(ProjectConfigViewModel model)
    //{
    //    if (!ModelState.IsValid)
    //        return View("Configure", model);

    //    _projectStore.SaveProjectConfig(model);
    //    return RedirectToAction("UploadAccessDb", "AccessUpload", new { area = "Admin" });
    //}

    //[HttpGet]
    //public IActionResult Configure(string projectName)
    //{
    //    var model = _projectStore.LoadProjectConfig(projectName) ?? new ProjectConfigurationViewModel { ProjectName = projectName };
    //    return View(model);
    //}
