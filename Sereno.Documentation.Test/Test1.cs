using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Sereno.Documentation.DataAccess;

namespace Sereno.Documentation
{
    [TestClass]
    public sealed class Test1
    {
        private string? _connectionString;


        [TestInitialize]
        public void Setup()
        {
            var configuration = ConfigurationHelper.GetConfiguration();
            _connectionString = configuration.GetConnectionString("TestDatabase");
        }


        [TestMethod]
        public void TestDatabaseConnection()
        {
            using var context = new AppDbContext(new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlServer(_connectionString)
                .Options);

            // Teste deine Datenbankoperationen
            Assert.IsNotNull(context);
        }
    }
}
