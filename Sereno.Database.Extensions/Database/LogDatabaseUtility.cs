using Microsoft.Data.SqlClient;
using System.Data.Common;
using System.Data;

namespace Sereno.Database
{
    public static class LogDatabaseUtility
    {

        public static void DropDatabaseWithLogDatabase(string connectionString, string? databaseName)
        {
            if (!String.IsNullOrWhiteSpace(databaseName))
            {
                string logDatabaseName = LogDatabaseUtility.GetLogDatabaseName(databaseName);

                DatabaseUtility.DropDatabase(connectionString, databaseName);
                DatabaseUtility.DropDatabase(connectionString, logDatabaseName);
            }
        }


        public static void CreateOrUpdateLogDatabase(string connectionString, string databaseName)
        {
            ArgumentNullException.ThrowIfNullOrEmpty(connectionString);
            ArgumentNullException.ThrowIfNullOrEmpty(databaseName);

            EnsureLogDatabaseExists(connectionString, databaseName);
            UpdateLogSchema(connectionString, databaseName);
        }


        public static string GetLogDatabaseName(string databaseName)
        {
            var logDatabaseName = $"{databaseName}Log";

            return logDatabaseName;
        }


        private static void UpdateLogSchema(string connectionString, string databaseName)
        {
            DataTable? mainTables = SchemaUtility.GetDatabaseTables(connectionString, databaseName);
            string logDatabaseName = GetLogDatabaseName(databaseName);

            if (mainTables != null)
            {
                DataTable? logTables = null;
                string logConnectionString = ConnectionStringUtility.ChangeDatabaseName(connectionString, logDatabaseName);
                using (var connection = new SqlConnection(logConnectionString))
                {
                    connection.Open();
                    logTables = connection.GetSchema("Tables");
                }

                var logTableDictionary = logTables.AsEnumerable()
                    .Where(row => !string.IsNullOrEmpty(row.Field<string>("TABLE_NAME")))
                    .ToDictionary(
                        row => row.Field<string>("TABLE_NAME")!,
                        row => row
                    );


                foreach (DataRow row in mainTables.Rows)
                {
                    string? tableName = row["TABLE_NAME"].ToString();

                    if (!logTableDictionary.ContainsKey(tableName!))
                    {
                        CreateLogTable(connectionString, databaseName, tableName!);
                    }
                    else
                    {
                        TableColumnsUpdate(connectionString, databaseName, tableName!);
                    }
                }
            }
        }

        private static void TableColumnsUpdate(string masterConnectionString, string databaseName, string tableName)
        {
            string mainConnectionString = ConnectionStringUtility.ChangeDatabaseName(masterConnectionString, databaseName);

            string logDatabaseName = GetLogDatabaseName(databaseName);
            string logConnectionString = ConnectionStringUtility.ChangeDatabaseName(masterConnectionString, logDatabaseName);


            // Hole das Schema der Tabelle aus der Hauptdatenbank
            using (var mainConnection = new SqlConnection(mainConnectionString))
            {
                mainConnection.Open();

                var mainTableSchema = mainConnection.GetSchema("Columns",
                [
                    null,
                    null,
                    tableName!,
                    null
                ]);

                // Hole das Schema der Tabelle aus der Logdatenbank
                using (var logConnection = new SqlConnection(logConnectionString))
                {
                    logConnection.Open();

                    var logTableSchema = logConnection.GetSchema("Columns",
                    [
                        null,
                        null,
                        tableName!,
                        null
                    ]);


                    // Erstelle ein Dictionary für die Log-Tabelle, um schnell Spalten zu prüfen
                    var logColumns = logTableSchema.AsEnumerable()
                        .ToDictionary(row => row.Field<string>("COLUMN_NAME")!, row => row);

                    foreach (DataRow mainColumn in mainTableSchema.Rows)
                    {
                        string columnName = mainColumn["COLUMN_NAME"].ToString()!;
                        string mainDataType = mainColumn["DATA_TYPE"].ToString()!;
                        int maxLength = mainColumn.Field<int?>("CHARACTER_MAXIMUM_LENGTH") ?? -1;
                        int precision = mainColumn.Field<int?>("NUMERIC_PRECISION") ?? 0;
                        int scale = mainColumn.Field<int?>("NUMERIC_SCALE") ?? 0;

                        if (!logColumns.TryGetValue(columnName, out DataRow? logColumn))
                        {
                            // Spalte existiert nicht in der Log-Tabelle -> hinzufügen
                            AddColumn(logConnection, tableName!, columnName, mainDataType, maxLength, precision, scale);
                        }
                        else
                        {
                            string logDataType = logColumn["DATA_TYPE"].ToString()!;
                            int logMaxLength = logColumn.Field<int?>("CHARACTER_MAXIMUM_LENGTH") ?? -1;
                            int logPrecision = logColumn.Field<int?>("NUMERIC_PRECISION") ?? 0;
                            int logScale = logColumn.Field<int?>("NUMERIC_SCALE") ?? 0;

                            if (!IsDataTypeEqual(mainDataType, maxLength, precision, scale, logDataType, logMaxLength, logPrecision, logScale))
                            {
                                UpdateColumnType(logConnection, tableName!, columnName, mainDataType, maxLength, precision, scale);
                            }
                        }
                    }
                }
            }
        }

        private static bool IsDataTypeEqual(
                    string mainDataType, int mainMaxLength, int mainPrecision, int mainScale,
                    string logDataType, int logMaxLength, int logPrecision, int logScale)
        {
            // Vergleiche die Datentypen und ihre Eigenschaften
            return mainDataType == logDataType &&
                   (mainDataType == "nvarchar" || mainDataType == "varchar" ? mainMaxLength == logMaxLength : true) &&
                   (mainDataType == "decimal" || mainDataType == "numeric" ? (mainPrecision == logPrecision && mainScale == logScale) : true);
        }

        private static void AddColumn(SqlConnection connection, string tableName, string columnName, string dataType, int maxLength, int precision, int scale)
        {
            using var command = connection.CreateCommand();
            string columnDefinition = SchemaUtility.GetColumnType(dataType, maxLength, precision, scale);
            command.CommandText = $"ALTER TABLE {tableName} ADD {columnName} {columnDefinition};";
            command.ExecuteNonQuery();
        }

        private static void UpdateColumnType(SqlConnection connection, string tableName, string columnName, string dataType, int maxLength, int precision, int scale)
        {
            using var command = connection.CreateCommand();
            string columnDefinition = SchemaUtility.GetColumnType(dataType, maxLength, precision, scale);
            command.CommandText = $"ALTER TABLE {tableName} ALTER COLUMN {columnName} {columnDefinition};";
            command.ExecuteNonQuery();
        }


        private static void EnsureLogDatabaseExists(string connectionString, string databaseName)
        {
            string logDatabaseName = GetLogDatabaseName(databaseName);
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using var command = connection.CreateCommand();
                command.CommandText = $@"
                    IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = @LogDatabaseName)
                    BEGIN
                        CREATE DATABASE {logDatabaseName};
                    END;";
                command.Parameters.Add(new SqlParameter("@LogDatabaseName", logDatabaseName));
                command.ExecuteNonQuery();
            }
        }


        private static void CreateLogTable(string masterConnectionString, string databaseName, string tableName)
        {
            string mainConnectionString = ConnectionStringUtility.ChangeDatabaseName(masterConnectionString, databaseName);

            string logDatabaseName = GetLogDatabaseName(databaseName);
            string logConnectionString = ConnectionStringUtility.ChangeDatabaseName(masterConnectionString, logDatabaseName);


            // Hole das Schema der Tabelle aus der Hauptdatenbank
            using (var mainConnection = new SqlConnection(mainConnectionString))
            {
                mainConnection.Open();

                // Einschränkungen: [catalog, schema, table, column]
                var restrictions = new string[4];
                restrictions[2] = tableName;

                var tableSchema = mainConnection.GetSchema("Columns", restrictions);

                // Sortiere die Spalten basierend auf der Reihenfolge in der Haupttabelle
                var orderedColumns = tableSchema.AsEnumerable()
                    .OrderBy(row => row.Field<int>("ORDINAL_POSITION"));

                var createTableSql = BuildCreateTableSql(tableName, orderedColumns);

                // Erstelle die Tabelle in der Logdatenbank
                using (var logConnection = new SqlConnection(logConnectionString))
                {
                    logConnection.Open();
                    using (var command = logConnection.CreateCommand())
                    {
                        command.CommandText = createTableSql;
                        command.ExecuteNonQuery();
                    }
                }
            }
        }

        private static string BuildCreateTableSql(string tableName, IEnumerable<DataRow> orderedColumns)
        {
            var sql = $"CREATE TABLE {tableName} (\n";

            // Füge die zusätzlichen Spalten hinzu
            sql += "    vChangeId nvarchar(50) NOT NULL,\n";
            sql += "    vChangeType nvarchar(10) NOT NULL,\n";
            sql += "    dChange datetime2 NOT NULL,\n";
            sql += "    vUserName nvarchar(400),\n";
            sql += "    tTimestamp timestamp NOT NULL,\n";

            // Füge die Spalten aus der Haupttabelle in der richtigen Reihenfolge hinzu
            foreach (var row in orderedColumns)
            {
                string? columnName = row["COLUMN_NAME"].ToString();
                string? dataType = row["DATA_TYPE"].ToString();
                bool isNullable = row["IS_NULLABLE"].ToString() == "YES";

                // Bestimme zusätzliche Details wie Länge, Präzision, Skalierung
                int maxLength = row.Field<int?>("CHARACTER_MAXIMUM_LENGTH") ?? -1;
                int precision = row.Field<int?>("NUMERIC_PRECISION") ?? 0;
                int scale = row.Field<int?>("NUMERIC_SCALE") ?? 0;

                sql += $"    {columnName} {SchemaUtility.GetColumnType(dataType!, maxLength, precision, scale)}";

                if (!isNullable)
                {
                    sql += " NOT NULL";
                }

                sql += ",\n";
            }

            // Entferne das letzte Komma und schließe die Definition
            sql = sql.TrimEnd(',', '\n') + "\n);";

            return sql;
        }

    }
}
