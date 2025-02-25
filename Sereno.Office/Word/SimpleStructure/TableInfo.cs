using System.Collections.Generic;
using System.Data;

namespace Sereno.Office.Word.SimpleStructure
{
    public class TableInfo
    {
        public bool HasHeader { get; set; }

        public List<ColumnInfo> Columns { get; set; } = new List<ColumnInfo>();

        public DataTable Data { get; set; }

    }
}
