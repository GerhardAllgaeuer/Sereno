using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;

namespace Sereno.Database
{
    public static class LogDatabaseUpdater
    {
        public static void UpdateLogDatabase<TContext>(TContext context, string mainConnectionString, Func<DbContextOptions<TContext>, TContext> contextFactory)
                where TContext : DbContext
        {
            ArgumentNullException.ThrowIfNull(context);
            if (string.IsNullOrWhiteSpace(mainConnectionString)) throw new ArgumentException("Connection string cannot be null or empty.", nameof(mainConnectionString));
            ArgumentNullException.ThrowIfNull(contextFactory);

            // Log-Datenbankname erstellen
            var mainDatabaseName = context.Database.GetDbConnection().Database ?? throw new InvalidOperationException("Main database name cannot be null.");
            var logDatabaseName = $"{mainDatabaseName}_Log";

            using (var mainConnection = new SqlConnection(mainConnectionString))
            {
                mainConnection.Open();
                using var command = mainConnection.CreateCommand();
                command.CommandText = $@"
                    IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = @LogDatabaseName)
                    BEGIN
                        CREATE DATABASE {logDatabaseName};
                    END;";
                command.Parameters.Add(new SqlParameter("@LogDatabaseName", logDatabaseName));
                command.ExecuteNonQuery();
            }

            // Verbindung zur Log-Datenbank
            var logConnectionString = mainConnectionString.Replace(mainDatabaseName, logDatabaseName);
            var logOptions = new DbContextOptionsBuilder<TContext>()
                .UseSqlServer(logConnectionString)
                .Options;

            using var logContext = contextFactory(logOptions);

            // Synchronisiere Tabellen in der Log-Datenbank
            foreach (var entityType in context.Model.GetEntityTypes())
            {
                var tableName = entityType.GetTableName()
                    ?? throw new InvalidOperationException("Table name cannot be null.");

                // Tabellenstruktur der Quelltabelle abrufen
                var columnDefinitions = GetColumnDefinitionsFromDatabase(mainConnectionString, tableName);

                // Tabelle in der Log-Datenbank erstellen, falls nicht vorhanden
                logContext.Database.ExecuteSqlRaw(
                    @"IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[' + @TableName + ']') AND type in (N'U'))
                      BEGIN
                          CREATE TABLE [dbo].[" + tableName + @"] (
                              " + columnDefinitions + @",
                              vChangeType NVARCHAR(10),
                              dChange DATETIME,
                              vUserName NVARCHAR(128)
                          );
                      END;",
                    new SqlParameter("@TableName", tableName)
                );

                // Zusätzliche Log-Spalten sicherstellen
                logContext.Database.ExecuteSqlRaw(
                    @"IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = @TableName AND COLUMN_NAME = 'vChangeType')
                      BEGIN
                          ALTER TABLE [dbo].[@TableName] ADD vChangeType NVARCHAR(10);
                      END;

                      IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = @TableName AND COLUMN_NAME = 'dChange')
                      BEGIN
                          ALTER TABLE [dbo].[@TableName] ADD dChange DATETIME;
                      END;

                      IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = @TableName AND COLUMN_NAME = 'vUserName')
                      BEGIN
                          ALTER TABLE [dbo].[@TableName] ADD vUserName NVARCHAR(128);
                      END;",
                    new SqlParameter("@TableName", tableName)
                );
            }
        }

        private static string GetColumnDefinitionsFromDatabase(string connectionString, string tableName)
        {
            ArgumentNullException.ThrowIfNull(connectionString);
            ArgumentNullException.ThrowIfNull(tableName);

            var columnDefinitions = new List<string>();

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using var command = connection.CreateCommand();
                command.CommandText = @"
                    SELECT 
                        COLUMN_NAME,
                        DATA_TYPE,
                        CHARACTER_MAXIMUM_LENGTH,
                        IS_NULLABLE
                    FROM INFORMATION_SCHEMA.COLUMNS
                    WHERE TABLE_NAME = @TableName;";
                command.Parameters.Add(new SqlParameter("@TableName", tableName));

                using var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    var columnName = reader["COLUMN_NAME"].ToString();
                    var dataType = reader["DATA_TYPE"].ToString();
                    var isNullable = reader["IS_NULLABLE"].ToString() == "YES" ? "NULL" : "NOT NULL";

                    // Prüfen, ob ein Längenattribut vorhanden ist (z. B. für VARCHAR, NVARCHAR)
                    var maxLength = reader["CHARACTER_MAXIMUM_LENGTH"] as int?;
                    var lengthDefinition = maxLength.HasValue && maxLength > 0 ? $"({maxLength})" : string.Empty;

                    columnDefinitions.Add($"[{columnName}] {dataType}{lengthDefinition} {isNullable}");
                }
            }

            return string.Join(", ", columnDefinitions);
        }
    }
}
