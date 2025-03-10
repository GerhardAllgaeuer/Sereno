using Dapper;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Sereno.Test.Database
{
    public static class DatabaseExtensions
    {
        public static DatabaseTableAssertion Should(this SqlConnection connection)
        {
            return new DatabaseTableAssertion(connection);
        }

        public static DatabaseRowAssertion DataRow(this SqlConnection connection, string table, object primaryKeyValue, string primaryKeyColumn = "vId")
        {
            return new DatabaseRowAssertion(connection, table, primaryKeyColumn, primaryKeyValue);
        }

        public static DatabaseRowsAssertion DataRows(this SqlConnection connection, string table, object whereClause = null, string orderBy = null)
        {
            if (connection.State != System.Data.ConnectionState.Open)
            {
                connection.Open();
            }

            var parameters = new DynamicParameters();

            string query = $"SELECT * FROM [{table}]\n";

            if (whereClause != null)
            {
                query += $"WHERE {whereClause}\n";
            }

            // Falls `orderBy` angegeben ist, ergänze die Query um `ORDER BY`
            if (!string.IsNullOrWhiteSpace(orderBy))
            {
                query += $"ORDER BY {orderBy}\n";
            }

            var result = connection.Query<dynamic>(query, parameters).ToList();

            var rows = result.Select(row => (IDictionary<string, object>)row)
                             .Select(dict => new Dictionary<string, object>(dict))
                             .ToList();

            return new DatabaseRowsAssertion(rows);
        }

        public static DatabaseRowsAssertion DataRows(this SqlConnection connection, string selectStatement)
        {
            if (connection.State != System.Data.ConnectionState.Open)
            {
                connection.Open();
            }

            var result = connection.Query<dynamic>(selectStatement).ToList();

            var rows = result.Select(row => (IDictionary<string, object>)row)
                             .Select(dict => new Dictionary<string, object>(dict))
                             .ToList();

            return new DatabaseRowsAssertion(rows);
        }
    }
}
