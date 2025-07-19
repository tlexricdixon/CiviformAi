using Entities;

namespace Repository;

public class TempAccessSchemaStore
{
    private readonly Dictionary<string, TableSchema> _tables = new();

    public void SaveSchema(List<TableSchema> tables)
    {
        _tables.Clear();
        foreach (var t in tables)
            _tables[t.TableName] = t;
    }

    public TableSchema? GetTable(string tableName)
    {
        _tables.TryGetValue(tableName, out var schema);
        return schema;
    }

    public List<TableSchema> GetAll() => _tables.Values.ToList();
    public void UpdateSchema(TableSchema table)
    {
        if (table == null || string.IsNullOrWhiteSpace(table.TableName) || string.IsNullOrWhiteSpace(table.FileName))
            return;

        var key = $"{table.FileName}::{table.TableName}";

        if (_tables.TryGetValue(key, out var existing))
        {
            existing.Columns = table.Columns ?? existing.Columns;
            existing.IsConverted = table.IsConverted;
        }
        else
        {
            _tables[key] = table;
        }
    }
}
