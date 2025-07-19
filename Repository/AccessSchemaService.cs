using Contracts;
using Entities;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace Repository;

public class AccessSchemaService : IAccessSchemaService
{
    public async Task<List<TableSchema>> CreateAccessSchemaAsync(List<IFormFile> files, string basePath)
    {
        var result = new List<TableSchema>();
        var uploadDir = Path.Combine(basePath, "TempUploads");

        Directory.CreateDirectory(uploadDir);

        foreach (var file in files)
        {
            var extension = Path.GetExtension(file.FileName);
            if (extension != ".accdb" && extension != ".mdb")
                continue;

            var savePath = Path.Combine(uploadDir, Path.GetRandomFileName() + extension);

            await using (var stream = file.OpenReadStream())
            await using (var fs = new FileStream(savePath, FileMode.Create))
            {
                await stream.CopyToAsync(fs);
            }

            await Task.Delay(50); // ensure OS releases file

            var reader = new AccessSchemaReader();
            var tables = reader.GetTableNames(savePath);

            foreach (var table in tables)
            {
                var columnList = reader.GetTableSchema(savePath, table);
                var columnSchemas = columnList.Select(col => new ColumnSchema
                {
                    ColumnName = col.ColumnName,
                    DataType = col.DataType?.ToString() ?? "UNKNOWN",
                    SqlType = col.SqlType
                }).ToList();

                result.Add(new TableSchema
                {
                    TableName = table,
                    Columns = columnSchemas,
                    FileName = Path.GetFileName(savePath)
                });
            }
        }

        return result;
    }

    public async Task SaveAccessSchemaAsync(List<TableSchema> schema, string basePath)
    {
        var schemaPath = Path.Combine(basePath, "schema.json");
        var schemaJson = JsonSerializer.Serialize(schema, new JsonSerializerOptions { WriteIndented = true });
        await File.WriteAllTextAsync(schemaPath, schemaJson);
    }

    public List<TableSchema> LoadAccessSchema(string basePath)
    {
        var schemaPath = Path.Combine(basePath, "schema.json");
        if (!File.Exists(schemaPath)) return new List<TableSchema>();

        var json = File.ReadAllText(schemaPath);
        return JsonSerializer.Deserialize<List<TableSchema>>(json) ?? new List<TableSchema>();
    }
}
