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

                    Dictionary<string, string> replacements = new Dictionary<string, string>
                    {
                        { "TableName", tableName! },
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
