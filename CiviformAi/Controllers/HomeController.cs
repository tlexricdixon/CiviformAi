using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Text.Json;
using CiviformAi.Models;
using Entities;

namespace CiviformAi.Controllers;
public class HomeController : Controller
{
    private readonly IWebHostEnvironment _env;

    public HomeController(IWebHostEnvironment env)
    {
        _env = env;
    }

    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> StartProject(ProjectConfigViewModel model)
    {
        if (!ModelState.IsValid)
            return View("Index", model);

        var projectPath = Path.Combine(_env.ContentRootPath, "Projects", model.ProjectName);
        Directory.CreateDirectory(projectPath);

        // Save config as JSON
        var configPath = Path.Combine(projectPath, "config.json");
        var config = new
        {
            model.ProjectName,
            model.DbServer,
            model.DatabaseName,
            model.DbUser,
            model.DbPassword,
            model.AccessVersion
        };

        await System.IO.File.WriteAllTextAsync(configPath, JsonSerializer.Serialize(config, new JsonSerializerOptions
        {
            WriteIndented = true
        }));

        // Save uploaded Access file
        if (model.AccessFile != null && model.AccessFile.Length > 0)
        {
            var accdbPath = Path.Combine(projectPath, "data.accdb");
            using var stream = new FileStream(accdbPath, FileMode.Create);
            await model.AccessFile.CopyToAsync(stream);
        }

        return RedirectToAction("Dashboard", "Project", new { projectName = model.ProjectName });
    }
}

