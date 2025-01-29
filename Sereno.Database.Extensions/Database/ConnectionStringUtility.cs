using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sereno.Database
{
    public class ConnectionStringInfo
    {
        public string? Server { get; set; }
        public string? Database { get; set; }
        public string? User { get; set; }
        public string? Password { get; set; }

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
                Server = GetValue(builder, new[] { "Server", "Data Source", "Addr", "Address", "Network Address" }),
                Database = GetValue(builder, new[] { "Initial Catalog", "Database" }),
                User = GetValue(builder, new[] { "User ID", "UID", "Username" }),
                Password = GetValue(builder, new[] { "Password", "PWD" })
            };

            return result;
        }

        private static string? GetValue(DbConnectionStringBuilder builder, string[] keys)
        {
            foreach (var key in keys)
            {
                if (builder.TryGetValue(key, out var value))
                {
                    return value?.ToString();
                }
            }
            return null;
        }
    }
}
