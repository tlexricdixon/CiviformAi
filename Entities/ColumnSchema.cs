namespace Entities;

public class ColumnSchema
{
    public string ColumnName { get; set; }
    public string DataType { get; set; } // string for simplicity, can map if needed
    public string SqlType { get; set; } // Optional, if you want to store SQL type directly
    public string Description { get { return string.Format("{0} {1} {2}", ColumnName ?? "UNKNOWN", DataType ?? "UNKNOWN", SqlType ?? "UNKNOWN"); } }
}
