using Contracts;
using Entities;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace Repository;

public class AccessFileService(IAccessSchemaService schemaService) : IAccessFileService
{
    private readonly IAccessSchemaService _schemaService = schemaService;

    public async Task SaveAccessFileAsync(IFormFile accessFile, string project, string rootPath)
    {
        if (accessFile == null || accessFile.Length <= 0)
            return;

        var basePath = Path.Combine(rootPath, "Projects", project);
        Directory.CreateDirectory(basePath);

        var accdbPath = Path.Combine(basePath, "data.accdb");
        var configPath = Path.Combine(basePath, "config.json");

        var fileName = Path.GetFileName(accessFile.FileName);

        using var fs = new FileStream(accdbPath, FileMode.Create);
        await accessFile.CopyToAsync(fs);

        if (File.Exists(configPath))
        {
            var json = File.ReadAllText(configPath);
            var config = JsonSerializer.Deserialize<ProjectSettings>(json);
            if (config != null)
            {
                config.AccessFileName = fileName;
                File.WriteAllText(configPath, JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true }));
            }
        }

        var schema = await _schemaService.CreateAccessSchemaAsync(new List<IFormFile> { accessFile }, basePath);
        await _schemaService.SaveAccessSchemaAsync(schema, basePath);
    }

}
