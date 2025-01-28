using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Sereno.Utilities;
using System.Data;

namespace Sereno.Database
{
    public class TriggerUtility
    {
        public static void CreateDefaultValuesTriggers(string connectionString)
        {
            DirectoryInfo solutionDirectory = CodeUtility.GetSolutionDirectory();
            DirectoryInfo templateDirectory = new DirectoryInfo(solutionDirectory.FullName + @"\Sereno.Database.Extensions\Database\Templates");

            string triggerTemplate = File.ReadAllText($@"{templateDirectory.FullName}\Trigger.sql");

            DataTable? mainTables = SchemaUtility.GetDatabaseTables(connectionString);

            if (mainTables != null)
            {
                foreach (DataRow tableRow in mainTables.Rows)
                {
                    string? tableName = tableRow["TABLE_NAME"].ToString();

                    DataTable columnsTable = SchemaUtility.GetTableColuns(connectionString, tableName!);

                    HashSet<string> defaultColumns = ["vId", "dCreate", "vCreateUser", "dModify", "vModifyUser"];

                    string columnsWithType = SchemaColumnBuilder.Build(new SchemaColumnBuilderParameters()
                    {
                        Columns = columnsTable,
                        BuilderType = SchemaColumnBuilderType.ColumnsWithDatatype,
                        ExcludeColumns = defaultColumns,
                        Spaces = 8,
                    });


                    // ohne Id und ohne Create/Update Columns
                    string dataColumns = SchemaColumnBuilder.Build(new SchemaColumnBuilderParameters()
                    {
                        Columns = columnsTable,
                        Spaces = 8,
                        ExcludeColumns = defaultColumns,
                    });                    

                    string dataColumnsWithPd = SchemaColumnBuilder.Build(new SchemaColumnBuilderParameters()
                    {
                        Columns = columnsTable,
                        Prefix = "pd",
                        Spaces = 8,
                        ExcludeColumns = defaultColumns,
                    });

                    string updateColumns = SchemaColumnBuilder.Build(new SchemaColumnBuilderParameters()
                    {
                        Columns = columnsTable,
                        BuilderType = SchemaColumnBuilderType.Update,
                        Prefix = "d",
                        UpdatePrefix = "pd",
                        Spaces = 8,
                        ExcludeColumns = defaultColumns,
                    });

                    Dictionary<string, string> replacements = new Dictionary<string, string>
                    {
                        { "TableName", tableName! },

                        { "ColumnsWithType", columnsWithType },
                        { "DataColumnsWithPd", dataColumnsWithPd },
                        { "DataColumns", dataColumns },
                        { "UpdateColumns", updateColumns },
                    };

                    string sqlFile = ReplaceVariables(triggerTemplate, replacements);
                }

            }
        }


        public static string ReplaceVariables(string template, Dictionary<string, string> replacements)
        {
            string result = template;

            foreach (var replacement in replacements)
            {
                result = result.Replace($"{{{{{replacement.Key}}}}}", replacement.Value);
            }

            return result;
        }

    }
}
