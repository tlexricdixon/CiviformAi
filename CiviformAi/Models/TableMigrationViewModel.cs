using Entities;
using Repository;

namespace CiviformAi.Models;

internal class TableMigrationViewModel
{
    public string TableName { get; set; }
    public string FileName { get; set; }
    public List<ColumnSchema> Columns { get; set; }
    public List<Dictionary<string, object>> Records { get; set; }
    public bool IsConverted { get; set; }  // 👈 Add this
}

