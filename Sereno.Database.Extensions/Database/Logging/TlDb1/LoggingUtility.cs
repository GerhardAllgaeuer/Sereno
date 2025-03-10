﻿using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Sereno.Utilities;
using System.Data;
using System.Data.Common;

namespace Sereno.Database.Logging.TlDb1
{
    public class LoggingUtility
    {
        public static void SetSessionContext(Context context, DbConnection connection)
        {
            SetSessionContextInternal(context, connection, false).GetAwaiter().GetResult();
        }

        public static async Task SetSessionContextAsync(Context context, DbConnection connection)
        {
            await SetSessionContextInternal(context, connection, true);
        }

        private static async Task SetSessionContextInternal(Context context, DbConnection connection, bool isAsync)
        {
            if (connection.State == System.Data.ConnectionState.Closed)
            {
                await OpenConnectionAsync(connection, isAsync);
            }

            using var command = connection.CreateCommand();
            command.CommandText = "EXEC sp_set_session_context @key = @keyParam, @value = @valueParam";
            command.Parameters.Add(new SqlParameter("@keyParam", "UserName"));
            command.Parameters.Add(new SqlParameter("@valueParam", context.UserName));

            if (isAsync)
            {
                await command.ExecuteNonQueryAsync();
            }
            else
            {
                command.ExecuteNonQuery();
            }
        }

        private static async Task OpenConnectionAsync(DbConnection connection, bool isAsync)
        {
            if (isAsync)
            {
                await connection.OpenAsync();
            }
            else
            {
                connection.Open();
            }
        }



        /// <summary>
        /// Log Datenbank und Trigger erstellen / aktualisieren
        /// </summary>
        public static void EnableLogging(string connectionString, string databaseName)
        {
            LogDatabaseUtility.CreateOrUpdateLogDatabase(connectionString, databaseName);
            CreateLogTriggers(connectionString, databaseName);
        }





        /// <summary>
        /// Trigger fürs Logging erstellen (Default Values , dCreate, ...)
        /// </summary>
        private static void CreateLogTriggers(string connectionString, string databaseName)
        {
            string logDatabaseName = LogDatabaseUtility.GetLogDatabaseName(databaseName);
            string triggerTemplate = LogDatabaseUtility.GetTemplate("LogTrigger.sql", "TlDb1");

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
