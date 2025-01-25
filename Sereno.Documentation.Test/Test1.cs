using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Sereno.Documentation.DataAccess;
using Sereno.Documentation.DataAccess.Entities;

namespace Sereno.Documentation
{
    [TestClass]
    public sealed class Test1
    {
        private string? connectionString;


        [TestInitialize]
        public void Setup()
        {
            var configuration = ConfigurationHelper.GetConfiguration();
            connectionString = configuration.GetConnectionString("Documentation_ConnectionString");
        }


        [TestMethod]
        public void Config_DatabaseCreate_Auto()
        {
            using var context = new AppDbContext(new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlServer(connectionString)
                .Options);

            // Erstelle die Datenbank, falls sie nicht existiert
            context.Database.EnsureDeleted();

            context.Database.Migrate();


            Func<DbContextOptions<AppDbContext>, AppDbContext> logContextFactory = logOptions =>
                new AppDbContext(logOptions);

            context.InitializeLogDatabase(connectionString, logContextFactory);

            List<Document> set = context.Documents.ToList();

            // Teste deine Datenbankoperationen
            Assert.IsNotNull(context);
        }
    }
}
