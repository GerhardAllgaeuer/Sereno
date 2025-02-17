using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Sereno.Utilities;
using System.Data;
using System.Data.Common;
using System.Net.NetworkInformation;

namespace Sereno.Database.ChangeTracking.TlDb1
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
        /// Verzeichnis, in dem die Templates liegen
        /// </summary>
        private static DirectoryInfo GetTemplateDirectory()
        {
            DirectoryInfo solutionDirectory = CodeUtility.GetSolutionDirectory();
            DirectoryInfo templateDirectory = new DirectoryInfo(solutionDirectory.FullName + @"\Sereno.Database.Extensions\Database\ChangeTracking\TlDb1\Templates");

            return templateDirectory;
        }

        /// <summary>
        /// Inhalt eines Templates zurückliefern
        /// </summary>
        private static string GetTemplate(string template)
        {
            DirectoryInfo templateDirectory = GetTemplateDirectory();

            string result = File.ReadAllText($@"{templateDirectory.FullName}\{template}");

            return result;
        }


        /// <summary>
        /// Tracking Schema und Trigger aktivieren / aktualisieren
        /// </summary>
        public static void EnableTrackingAndCreateLogDatabase(string connectionString, string databaseName)
        {
            LogDatabaseUtility.UpdateLogDatabase(connectionString, databaseName);
            CreateLogTable(connectionString, databaseName);
            CreateChangeTrackingTriggers(connectionString, databaseName);
        }

        /// <summary>
        /// Tabelle, in welche alle Änderungen gesamt geschrieben werden, Basis für die Sync Verarbeitung
        /// </summary>
        private static void CreateLogTable(string connectionString, string databaseName)
        {
            ConnectionStringInfo connectionInfo = ConnectionStringUtility.ParseConnectionString(connectionString);

            string logDatabaseName = LogDatabaseUtility.GetLogDatabaseName(databaseName);
            string sql = GetTemplate("LogTable.sql");

            ScriptParameters scriptParameters = new()
            {
                ServerName = connectionInfo.Server!,
                DatabaseName = logDatabaseName,
                UserName = connectionInfo.User!,
                Password = connectionInfo.Password!,
                ScriptContent = sql,
            };

            ScriptUtility.ExecuteDatabaseScript(scriptParameters);
        }




        /// <summary>
        /// Trigger für ChangeTracking erstellen (Default Values , dCreate, ...)
        /// </summary>
        private static void CreateChangeTrackingTriggers(string connectionString, string databaseName)
        {
            string logDatabaseName = LogDatabaseUtility.GetLogDatabaseName(databaseName);
            string triggerTemplate = GetTemplate("ChangeTrackingTrigger.sql");

            DataTable? mainTables = SchemaUtility.GetDatabaseTables(connectionString, databaseName);

            if (mainTables != null)
            {
                foreach (DataRow tableRow in mainTables.Rows)
                {
                    string? tableName = tableRow["TABLE_NAME"].ToString();

                    DataTable columnsTable = SchemaUtility.GetTableColumns(connectionString, databaseName, tableName!);

                    HashSet<string> defaultColumns = ["vId", "dCreate", "vCreateUser", "dModify", "vModifyUser"];

                    // ohne Id und ohne Create/Update Columns
                    string dataColumns = SchemaColumnBuilder.Build(new SchemaColumnBuilderParameters()
                    {
                        Columns = columnsTable,
                        Spaces = 28,
                        ExcludeColumns = defaultColumns,
                    });

                    Dictionary<string, string> replacements = new()
                    {
                        { "TableName", tableName! },
                        { "DataColumns", dataColumns },
                        { "LogDatabaseName", logDatabaseName },
                    };

                    string sql = ReplaceVariables(triggerTemplate, replacements);

                    ConnectionStringInfo connectionInfo = ConnectionStringUtility.ParseConnectionString(connectionString);
                    ScriptParameters scriptParameters = new()
                    {
                        ServerName = connectionInfo.Server,
                        DatabaseName = databaseName,
                        UserName = connectionInfo.User,
                        Password = connectionInfo.Password,
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
