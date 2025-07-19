using CiviformAi.Models;
using Contracts;
using Entities;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Microsoft.AspNetCore.Hosting;

namespace CiviformAi.Areas.Project.Controllers;

[Area("Project")]
public class DashboardController : Controller
{
    private readonly IWebHostEnvironment _env;
    private readonly IAccessFileService _accessFileService;

    public DashboardController(IWebHostEnvironment env, IAccessFileService accessFileService)
    {
        _env = env;
        _accessFileService = accessFileService;
    }
    public IActionResult Index(string project)
    {
        if (string.IsNullOrWhiteSpace(project))
            return RedirectToAction("Index", "Project", new { area = "Admin" });

        var configPath = Path.Combine(_env.ContentRootPath, "Projects", project, "config.json");
        if (!System.IO.File.Exists(configPath))
            return NotFound($"Project configuration for '{project}' not found.");

        var json = System.IO.File.ReadAllText(configPath);
        var config = JsonSerializer.Deserialize<ProjectSettings>(json);
        if (config == null)
            return NotFound("Project configuration could not be loaded.");

        ViewData["Title"] = $"Dashboard: {config.ProjectName}";
        return View(config);
    }
    [HttpPost]
    public async Task<IActionResult> UploadAccessFile(string project, IFormFile AccessFile)
    {
        var path = Path.Combine(_env.ContentRootPath, "Projects", project);
        var configPath = Path.Combine(path, "config.json");

        if (AccessFile != null && AccessFile.Length > 0)
        {
            await _accessFileService.SaveAccessFileAsync
                 (AccessFile, Path.Combine(path, "data.accdb"), configPath);
        } 

        TempData["Status"] = "Access file uploaded and configuration updated.";
        return RedirectToAction("Index", new { project });
    }
}
