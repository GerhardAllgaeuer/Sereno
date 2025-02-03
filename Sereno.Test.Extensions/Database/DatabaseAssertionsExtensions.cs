using Microsoft.Data.SqlClient;

namespace Sereno.Test.Database
{
    public static class DatabaseAssertionsExtensions
    {
        public static DatabaseAssertions Should(this SqlConnection connection)
        {
            return new DatabaseAssertions(connection);
        }
    }
}
