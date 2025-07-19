using Contracts;
using Entities;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace Repository;

public class AccessFileService : IAccessFileService
{
    private readonly Contracts.IAccessSchemaReader<TableSchema> _reader;

    public AccessFileService(IAccessSchemaReader<TableSchema> reader)
    {
        _reader = reader;
    }

    public async Task SaveAccessFileAsync(IFormFile accessFile, string project, string path)
    {
        if (accessFile == null || accessFile.Length <= 0)
            return;

        var basePath = Path.Combine(path, "Projects", project);
        Directory.CreateDirectory(basePath);

        var accdbPath = Path.Combine(basePath, "data.accdb");
        var configPath = Path.Combine(basePath, "config.json");
        var schemaPath = Path.Combine(basePath, "schema.json");

        var fileName = Path.GetFileName(accessFile.FileName);

        // Save the uploaded file
        using var fs = new FileStream(accdbPath, FileMode.Create);
        await accessFile.CopyToAsync(fs);

        // Update project config
        if (System.IO.File.Exists(configPath))
        {
            var json = System.IO.File.ReadAllText(configPath);
            var config = JsonSerializer.Deserialize<ProjectSettings>(json);

            if (config != null)
            {
                config.AccessFileName = fileName;

                var updatedJson = JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true });
                System.IO.File.WriteAllText(configPath, updatedJson);
            }
        }

        // Extract and save schema
        var schema = await _reader.GetAccessSchemaAsync(new List<IFormFile> { accessFile }, basePath);
        var schemaJson = JsonSerializer.Serialize(schema, new JsonSerializerOptions { WriteIndented = true });
        await System.IO.File.WriteAllTextAsync(schemaPath, schemaJson);
    }
}
