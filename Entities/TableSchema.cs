using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities;

public class TableSchema
{
    public string TableName { get; set; }
    public string FileName { get; set; } // 👈 Add this line
    public List<ColumnSchema> Columns { get; set; } = new List<ColumnSchema>();
    public bool IsConverted { get; set; }  // 👈 Add this
}
