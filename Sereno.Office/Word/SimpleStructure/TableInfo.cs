using System.Data;

namespace Sereno.Office.Word.SimpleStructure
{
    public class TableInfo
    {
        public bool HasHeader { get; set; }

        public List<ColumnInfo> Columns { get; set; } = [];

        public required DataTable Data { get; set; }

    }
}
