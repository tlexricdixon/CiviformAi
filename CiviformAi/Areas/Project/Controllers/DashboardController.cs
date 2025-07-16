using Microsoft.AspNetCore.Mvc;

namespace CiviformAi.Areas.Project.Controllers;

[Area("Project")]
public class DashboardController : Controller
{
    public IActionResult Index(string projectName)
    {
        // Show forms, queries, etc for this project
        ViewBag.Project = projectName;
        return View();
    }
}
