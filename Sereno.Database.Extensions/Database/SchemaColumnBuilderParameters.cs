using System.Data;

namespace Sereno.Database
{

    public enum SchemaColumnBuilderType
    {
        // vId nvarchar(50)
        ColumnsWithDatatype,

        // a.vId = b.vId
        ColumnsAsUpdate,

        // vId, vTitle, ..
        ColumnsList,
    }

    public class SchemaColumnBuilderParameters
    {
        public required DataTable Columns { get; set; }

        public string Prefix { get; set; } = "";

        public string UpdatePrefix { get; set; } = "";

        public SchemaColumnBuilderType BuilderType { get; set; } = SchemaColumnBuilderType.ColumnsList;

        public string Delimiter { get; set; } = ",";

        public int TabStops { get; set; }

        public int Spaces { get; set; }

        public HashSet<string> ExcludeColumns { get; set; } = [];


    }
}
