using Contracts;
using Entities;
using Microsoft.AspNetCore.Http;
using System.Reflection.PortableExecutable;
using System.Text.Json;

namespace Repository;

public class AccessFileService(IAccessSchemaReader<TableSchema> reader, IAccessSchemaService schemaService) : IAccessFileService
{
    private readonly IAccessSchemaReader<TableSchema> _reader = reader;
    private readonly IAccessSchemaService _schemaService = schemaService;

    public async Task SaveAccessFileAsync(IFormFile accessFile, string project, string rootPath)
    {
        if (accessFile == null || accessFile.Length <= 0)
            return;
        
        
        var basePath = Path.Combine(rootPath, "Projects", project);
        Directory.CreateDirectory(basePath);

        var accdbPath = Path.Combine(basePath, accessFile.FileName);
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

        var schema = _reader.GetAccessSchema(accdbPath);
        
        await _schemaService.SaveAccessSchemaAsync(schema, basePath);
    }

}
