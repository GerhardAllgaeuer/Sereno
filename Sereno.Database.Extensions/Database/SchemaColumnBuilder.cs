using Azure.Core;
using Sereno.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sereno.Database
{
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

                        case SchemaColumnBuilderType.ColumnsAsUpdate:
                            stringbuilder.AppendLine($"{spaces}{tabs}{prefix}{columnName} = {updatePrefix}{columnName},");
                            break;

                        default:
                            stringbuilder.AppendLine($"{spaces}{tabs}{prefix}{columnName},");
                            break;
                    }

                }
            }

            result = stringbuilder.ToString();

            result = StringUtility.RemoveLastCharacter(result, ",");
            result = StringUtility.RemoveLastLineBreak(result);

            return result;
        }

    }
}
