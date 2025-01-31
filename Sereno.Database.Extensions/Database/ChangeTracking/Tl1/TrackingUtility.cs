using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Sereno.Utilities;
using System.Data;
using System.Data.Common;

namespace Sereno.Database.ChangeTracking.Tl1
{
    public class TrackingUtility
    {
        public static void SetSessionContext(Context context, DbConnection connection)
        {
            if (connection.State == System.Data.ConnectionState.Closed)
            {
                connection.Open();
            }

            using var command = connection.CreateCommand();
            command.CommandText = "EXEC sp_set_session_context @key = @keyParam, @value = @valueParam";
            command.Parameters.Add(new SqlParameter("@keyParam", "UserName"));
            command.Parameters.Add(new SqlParameter("@valueParam", context.UserName));
            command.ExecuteNonQuery();
        }

        public static async Task SetSessionContextAsync(Context context, DbConnection connection)
        {
            if (connection.State == System.Data.ConnectionState.Closed)
            {
                connection.Open();
            }

            using var command = connection.CreateCommand();
            command.CommandText = "EXEC sp_set_session_context @key = @keyParam, @value = @valueParam";
            command.Parameters.Add(new SqlParameter("@keyParam", "UserName"));
            command.Parameters.Add(new SqlParameter("@valueParam", context.UserName));
            await command.ExecuteNonQueryAsync();
        }


        /// <summary>
        /// Entities so konfigurieren, damit Trigger zugelassen werden
        /// </summary>
        public static void EnableTriggersOnTables(ModelBuilder modelBuilder)
        {
            // Für alle Entitäten im Modell durchlaufen
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                // Setze die ID-Eigenschaft auf ValueGeneratedNever, wenn eine ID existiert
                var primaryKey = entityType.FindPrimaryKey();
                if (primaryKey != null)
                {
                    foreach (var keyProperty in primaryKey.Properties)
                    {
                        //modelBuilder.Entity(entityType.ClrType)
                        //    .Property(keyProperty.Name)
                        //    .ValueGeneratedNever();
                    }
                }

                // Deaktiviere die OUTPUT-Klausel für alle Tabellen mit Triggern
                modelBuilder.Entity(entityType.ClrType)
                    .ToTable(tb => tb.UseSqlOutputClause(false));
            }
        }



        /// <summary>
        /// Trigger für Default Values erstellen (dCreate, ...)
        /// </summary>
        public static void CreateDefaultValuesTriggers(string connectionString)
        {
            DirectoryInfo solutionDirectory = CodeUtility.GetSolutionDirectory();
            DirectoryInfo templateDirectory = new DirectoryInfo(solutionDirectory.FullName + @"\Sereno.Database.Extensions\Database\ChangeTracking\Tl1\Templates");

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

                    Dictionary<string, string> replacements = new()
                    {
                        { "TableName", tableName! },

                        { "ColumnsWithType", columnsWithType },
                        { "DataColumnsWithPd", dataColumnsWithPd },
                        { "DataColumns", dataColumns },
                        { "UpdateColumns", updateColumns },
                    };

                    string sql = ReplaceVariables(triggerTemplate, replacements);

                    ConnectionStringInfo connectionInfo = ConnectionStringUtility.ParseConnectionString(connectionString);
                    ScriptParameters scriptParameters = new()
                    {
                        ServerName = connectionInfo.Server!,
                        DatabaseName = connectionInfo.Database!,
                        UserName = connectionInfo.User!,
                        Password = connectionInfo.Password!,
                        ScriptContent = sql,
                    };

                    ScriptUtility.ExecuteDatabaseScript(scriptParameters);
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
