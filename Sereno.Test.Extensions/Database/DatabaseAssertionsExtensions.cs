using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Sereno.Test.Database
{
    public static class DatabaseAssertionsExtensions
    {
        public static DatabaseAssertions Should(this SqlConnection connection)
        {
            return new DatabaseAssertions(connection);
        }

        public static DataRowAssertion DataRow(this SqlConnection connection, string table, object primaryKeyValue, string primaryKeyColumn = "vId")
        {
            return new DataRowAssertion(connection, table, primaryKeyColumn, primaryKeyValue);
        }

        public static DataRowsAssertion DataRows(this SqlConnection connection, string table, object? whereClause = null, string? orderBy = null)
        {
            if (connection.State != System.Data.ConnectionState.Open)
            {
                connection.Open();
            }

            var whereConditions = "";
            var parameters = new DynamicParameters();

            if (whereClause != null)
            {
                var props = whereClause.GetType().GetProperties();
                whereConditions = "WHERE " + string.Join(" AND ", props.Select(p => $"[{p.Name}] = @{p.Name}"));
                foreach (var prop in props)
                {
                    parameters.Add($"@{prop.Name}", prop.GetValue(whereClause));
                }
            }

            string query = $"SELECT * FROM [{table}] {whereConditions}";

            // Falls `orderBy` angegeben ist, ergänze die Query um `ORDER BY`
            if (!string.IsNullOrWhiteSpace(orderBy))
            {
                query += $" ORDER BY {orderBy}";
            }

            var result = connection.Query<dynamic>(query, parameters).ToList();

            var rows = result.Select(row => (IDictionary<string, object>)row)
                             .Select(dict => new Dictionary<string, object>(dict))
                             .ToList();

            return new DataRowsAssertion(rows);
        }
    }
}
