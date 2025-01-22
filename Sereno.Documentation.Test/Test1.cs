using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Sereno.Documentation.DataAccess;
using Sereno.Documentation.DataAccess.Entities;

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

            // Erstelle die Datenbank, falls sie nicht existiert
            context.Database.EnsureCreated();


            List<Document> set = context.Documents.ToList();

            // Teste deine Datenbankoperationen
            Assert.IsNotNull(context);
        }
    }
}
