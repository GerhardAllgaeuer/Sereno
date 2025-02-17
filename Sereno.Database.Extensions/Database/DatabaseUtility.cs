using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sereno.Database
{
    public class DatabaseUtility
    {

        /// <summary>
        /// Datenbank löschen
        /// </summary>
        public static void DropDatabase(string connectionString, string? databaseName = null)
        {
            if (connectionString != null)
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    if (String.IsNullOrWhiteSpace(databaseName))
                    {
                        ConnectionStringInfo connectionInfo = ConnectionStringUtility.ParseConnectionString(connectionString);

                        if (connectionInfo != null &&
                            !String.IsNullOrWhiteSpace(connectionInfo.Database))
                        {
                            databaseName = connectionInfo.Database;
                        }
                    }

                    if (String.IsNullOrWhiteSpace(databaseName))
                    {
                        throw new Exception("Database could not be determined");
                    }

                    connection.Open();

                    using var command = connection.CreateCommand();
                    command.CommandText = $@"
                            
                            IF EXISTS (SELECT * FROM sys.databases WHERE name = @DatabaseName)
                            BEGIN
                                ALTER DATABASE [{databaseName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
                                DROP DATABASE {databaseName};
                            END;";

                    command.Parameters.Add(new SqlParameter("@DatabaseName", databaseName));

                    command.ExecuteNonQuery();
                }
            }
        }

        public static void TruncateTables(SqlConnection? connection, params string[] tables)
        {
            if (connection == null)
                throw new ArgumentNullException(nameof(connection));

            foreach (var table in tables)
            {
                using var cmd = new SqlCommand(
                    $"DELETE [{table}]"
                    , connection);

                cmd.ExecuteNonQuery();
            }
        }

        public static void ResetIdentity(SqlConnection? connection, string table)
        {
            if (connection == null)
                throw new ArgumentNullException(nameof(connection));

            using var cmd = new SqlCommand(
                $"DBCC CHECKIDENT ('[{table}]', RESEED, 0)"
                , connection);

            cmd.ExecuteNonQuery();
        }
    }
}
