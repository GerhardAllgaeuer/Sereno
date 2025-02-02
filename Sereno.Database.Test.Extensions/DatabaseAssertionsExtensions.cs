using Microsoft.Data.SqlClient;

namespace FluentAssertions.DapperExtensions
{
    public static class DatabaseAssertionsExtensions
    {
        public static DatabaseAssertions Should(this SqlConnection connection)
        {
            return new DatabaseAssertions(connection);
        }
    }
}
