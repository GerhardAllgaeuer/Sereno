using System.Data.Common;

namespace Sereno.Database
{
    public class ConnectionStringInfo
    {
        public string Server { get; set; } = string.Empty;

        public string Database { get; set; } = string.Empty;
        public string User { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;

        public override string ToString()
        {
            return $"Server: {Server}, Database: {Database}, User: {User}, Password: {(string.IsNullOrEmpty(Password) ? "******" : "Set")}";
        }
    }


    public class ConnectionStringUtility
    {
        public static ConnectionStringInfo ParseConnectionString(string connectionString)
        {
            var builder = new DbConnectionStringBuilder
            {
                ConnectionString = connectionString
            };

            var result = new ConnectionStringInfo
            {
                Server = GetValue(builder, ["Server", "Data Source", "Addr", "Address", "Network Address"]),
                Database = GetValue(builder, ["Initial Catalog", "Database"]),
                User = GetValue(builder, ["User ID", "UID", "Username"]),
                Password = GetValue(builder, ["Password", "PWD"])
            };

            return result;
        }


        public static string ChangeDatabaseName(string connectionString, string newDatabaseName)
        {
            var builder = new DbConnectionStringBuilder { ConnectionString = connectionString };

            if (builder.ContainsKey("Database"))
            {
                builder["Database"] = newDatabaseName;
            }
            else if (builder.ContainsKey("Initial Catalog")) // Alternative Bezeichnung für Database
            {
                builder["Initial Catalog"] = newDatabaseName;
            }
            else
            {
                throw new ArgumentException("Der ConnectionString enthält keine 'Database' oder 'Initial Catalog' Angabe.");
            }

            return builder.ConnectionString;
        }


        private static string GetValue(DbConnectionStringBuilder builder, string[] keys)
        {
            foreach (var key in keys)
            {
                if (builder.TryGetValue(key, out var value) && value is not null)
                {
                    return value.ToString() ?? string.Empty;
                }
            }

            return string.Empty;
        }

    }
}
