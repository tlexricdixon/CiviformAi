using Microsoft.AspNetCore.Mvc;

namespace CiviformAi.Controllers;

public class AccessUploadController : Controller
{
    private readonly IWebHostEnvironment _env;
    public AccessUploadController(IWebHostEnvironment env) => _env = env;
    [HttpGet("upload-access-db")]
    public IActionResult UploadAccessDb()
    {
        return View();
    }

    [HttpPost("upload-access-db")]
    [RequestSizeLimit(50_000_000)]
    public async Task<IActionResult> UploadAccessDb(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file uploaded.");

        var uploadDir = Path.Combine(_env.ContentRootPath, "TempUploads");
        Directory.CreateDirectory(uploadDir);

        var path = Path.Combine(uploadDir, Path.GetRandomFileName() + Path.GetExtension(file.FileName));
        await using var fs = new FileStream(path, FileMode.Create);
        await file.CopyToAsync(fs);

        ViewBag.Message = $"Uploaded: {file.FileName} ({file.Length / 1024} KB)";
        return View("UploadAccessDb");
    }
}
