using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Sereno.System.DataAccess;

namespace Sereno.System
{
    public static class DbContextFactory
    {
        public static AppDbContext CreateTestDb(Context appContext)
        {
            var configuration = ConfigurationUtility.GetConfiguration();
            string connectionString = configuration.GetConnectionString("TestDb_ConnectionString")!;

            return Create(appContext, connectionString);
        }


        public static AppDbContext Create(Context appContext, string connectionString)
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlServer(connectionString)
            .Options;

            var dbContext = new AppDbContext(options, appContext);
            return dbContext;
        }
    }
}
