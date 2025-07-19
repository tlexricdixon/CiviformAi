using CiviformAi.Extensions;
using CiviformAi.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace CiviformAi.Areas.Project.Controllers
{
    public class AccessFormsController(IWebHostEnvironment env) : Controller
    {
        private readonly string path = env.ContentRootPath;

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult ExportForms(string fileName)
        {
            var dbPath = Path.Combine(path, "TempUploads", fileName);
            var bridge = new AccessFormInteropBridge();
            var json = bridge.ExportForms(dbPath);

            var parsed = JsonConvert.DeserializeObject<List<FormDefinition>>(json); // Define a POCO if needed
            return View(parsed); // Or ViewBag it for fast testing
        }
    }
}
