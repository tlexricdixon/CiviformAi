using Contracts;
using Entities;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace Repository;

public class AccessSchemaService : IAccessSchemaService
{
    

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
