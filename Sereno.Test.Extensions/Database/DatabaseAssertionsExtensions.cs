using Microsoft.Data.SqlClient;

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
    }
}
