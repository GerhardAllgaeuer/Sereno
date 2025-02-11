using Microsoft.Data.SqlClient;
using System.Data.Common;
using System.Data;

namespace Sereno.Database
{
    public static class LogDatabaseUtility
    {
        public static void UpdateLogDatabase(string connectionString)
        {
            ArgumentNullException.ThrowIfNullOrEmpty(connectionString);

            EnsureLogDatabaseExists(connectionString);
            UpdateLogSchema(connectionString);
        }


        private static void UpdateLogSchema(string connectionString)
        {
            DataTable? mainTables = SchemaUtility.GetDatabaseTables(connectionString);

            if (mainTables != null)
            {
                DataTable? logTables = null;
                string logConnectionString = GetLogDatabaseConnectionString(connectionString);
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
                        CreateLogTable(connectionString, logConnectionString, tableName!);
                    }
                    else
                    {
                        TableColumnsUpdate(connectionString, logConnectionString, tableName!);
                    }
                }
            }
        }

        private static void TableColumnsUpdate(string mainConnectionString, string logConnectionString, string tableName)
        {
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




        public static void DeleteLogDatabase(string connectionString)
        {
            ArgumentNullException.ThrowIfNullOrEmpty(connectionString);

            string logDatabaseName = GetLogDatabaseName(connectionString);
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using var command = connection.CreateCommand();
                command.CommandText = $@"
                    IF EXISTS (SELECT * FROM sys.databases WHERE name = @LogDatabaseName)
                    BEGIN
                        DROP DATABASE {logDatabaseName};
                    END;";
                command.Parameters.Add(new SqlParameter("@LogDatabaseName", logDatabaseName));
                command.ExecuteNonQuery();
            }
        }

        private static void EnsureLogDatabaseExists(string connectionString)
        {
            string logDatabaseName = GetLogDatabaseName(connectionString);
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


        private static void CreateLogTable(string mainConnectionString, string logConnectionString, string tableName)
        {
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

        public static string GetLogDatabaseConnectionString(string connectionString)
        {
            string logDatabaseName = GetLogDatabaseName(connectionString);

            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentException("ConnectionString darf nicht leer oder null sein.", nameof(connectionString));

            if (string.IsNullOrWhiteSpace(logDatabaseName))
                throw new ArgumentException("Der neue Datenbankname darf nicht leer oder null sein.", nameof(logDatabaseName));

            // Verwende DbConnectionStringBuilder zum Parsen
            var builder = new DbConnectionStringBuilder
            {
                ConnectionString = connectionString
            };

            // Überprüfe und aktualisiere den Datenbanknamen
            if (builder.ContainsKey("Initial Catalog"))
            {
                builder["Initial Catalog"] = logDatabaseName;
            }
            else if (builder.ContainsKey("Database"))
            {
                builder["Database"] = logDatabaseName;
            }
            else
            {
                // Füge den Datenbanknamen hinzu, falls nicht vorhanden
                builder.Add("Initial Catalog", logDatabaseName);
            }

            return builder.ConnectionString;
        }

        private static string GetLogDatabaseName(string connectionString)
        {
            ConnectionStringInfo connectionInfo = ConnectionStringUtility.ParseConnectionString(connectionString);

            var logDatabaseName = $"{connectionInfo.Database}Log";

            return logDatabaseName;
        }
    }
}
