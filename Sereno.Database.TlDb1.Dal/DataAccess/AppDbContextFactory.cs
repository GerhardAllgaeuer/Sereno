using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;

namespace Sereno.TlDb1.DataAccess
{
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseSqlServer(@"Server=localhost\SQLExpress;Database=SerenoTlDb1Test;User Id=sa;Password=krxs8187;TrustServerCertificate=True;");

            Context context = new()
            {
                UserName = "",
            };

            return new AppDbContext(optionsBuilder.Options, context);
        }
    }
}
