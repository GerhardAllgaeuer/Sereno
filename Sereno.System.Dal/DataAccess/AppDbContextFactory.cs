using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;

namespace Sereno.Identity.DataAccess
{
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseSqlServer(@"Server=localhost\SQLExpress;Database=System;User Id=sa;Password=krxs8187;TrustServerCertificate=True;");

            Context context = new()
            {
                UserName = "",
            };

            return new AppDbContext(optionsBuilder.Options, context);
        }

        public static AppDbContext CreateDbContext(string connectionString)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseSqlServer(connectionString);

            Context context = new()
            {
                UserName = "sync@test.com",
            };

            return new AppDbContext(optionsBuilder.Options, context);
        }
    }
}
