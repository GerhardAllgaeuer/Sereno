using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Sereno.TlDb1.DataAccess;

namespace Sereno.Database.TlDb1.Test
{
    public static class TestDbContextFactory
    {
        public static AppDbContext Create(Context appContext)
        {
            var configuration = ConfigurationUtility.GetConfiguration();
            var connectionString = configuration.GetConnectionString("TestDb_ConnectionString")!;

            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlServer(connectionString)
            .Options;

            var dbContext = new AppDbContext(options, appContext);
            return dbContext;
        }
    }
}
