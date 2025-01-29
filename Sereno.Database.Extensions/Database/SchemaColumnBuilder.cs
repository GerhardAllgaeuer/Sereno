using Sereno.Utilities;
using System.Data;
using System.Text;

namespace Sereno.Database
{
    public enum SchemaColumnBuilderType
    {
        // vId nvarchar(50)
        ColumnsWithDatatype,

        // a.vId = b.vId
        Update,

        // vId, vTitle, ..
        OnlyColumns,
    }

    public class SchemaColumnBuilderParameters
    {
        public required DataTable Columns { get; set; }

        public string Prefix { get; set; } = "";

        public string UpdatePrefix { get; set; } = "";

        public SchemaColumnBuilderType BuilderType { get; set; } = SchemaColumnBuilderType.OnlyColumns;

        public string Delimiter { get; set; } = ",";

        public int TabStops { get; set; }

        public int Spaces { get; set; }

        public HashSet<string> ExcludeColumns { get; set; } = [];

        public bool RemoveLastComma { get; set; } = false;


    }


    public class SchemaColumnBuilder
    {
        public static string Build(SchemaColumnBuilderParameters parameters)
        {
            string result = string.Empty;
            var stringbuilder = new StringBuilder();
            string tabs = new('\t', parameters.TabStops);
            string spaces = new(' ', parameters.Spaces);

            string prefix = parameters.Prefix;
            if (!string.IsNullOrWhiteSpace(prefix))
            {
                prefix = prefix + ".";
            }

            string updatePrefix = parameters.UpdatePrefix;
            if (!string.IsNullOrWhiteSpace(updatePrefix))
            {
                updatePrefix = updatePrefix + ".";
            }

            foreach (DataRow row in parameters.Columns.Rows)
            {
                string? columnName = row["COLUMN_NAME"].ToString();

                if (!parameters.ExcludeColumns.Contains(columnName!))
                {
                    string? dataType = row["TYPE_FORMATTED"].ToString();

                    switch (parameters.BuilderType)
                    {
                        case SchemaColumnBuilderType.ColumnsWithDatatype:
                            stringbuilder.AppendLine($"{spaces}{tabs}{prefix}{columnName} {dataType},");
                            break;

                        case SchemaColumnBuilderType.Update:
                            stringbuilder.AppendLine($"{spaces}{tabs}{prefix}{columnName} = {updatePrefix}{columnName},");
                            break;

                        default:
                            stringbuilder.AppendLine($"{spaces}{tabs}{prefix}{columnName},");
                            break;
                    }

                }
            }

            result = stringbuilder.ToString();

            if (parameters.RemoveLastComma)
            {
                result = StringUtility.RemoveLastCharacter(result, ",");
            }

            result = StringUtility.RemoveLastLineBreak(result);

            return result;
        }

    }
}
