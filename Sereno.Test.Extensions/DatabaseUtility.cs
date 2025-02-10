using Microsoft.Data.SqlClient;
using Dapper;

namespace Sereno.Test.Database
{
    public static class DatabaseUtility
    {
        public static void TruncateTables(SqlConnection? connection, params string[] tables)
        {
            if (connection == null)
                throw new ArgumentNullException(nameof(connection));

            foreach (var table in tables)
            {
                connection.Execute($"TRUNCATE TABLE [{table}]");
            }
        }

        public static void ResetIdentity(SqlConnection? connection, string table)
        {
            if (connection == null)
                throw new ArgumentNullException(nameof(connection));

            connection.Execute($"DBCC CHECKIDENT ('[{table}]', RESEED, 0)");
        }
    }
}
