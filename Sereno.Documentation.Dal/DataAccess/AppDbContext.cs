using Microsoft.EntityFrameworkCore;
using Sereno.Database;
using Sereno.Database.Logging.TlDb1;
using Sereno.Documentation.DataAccess.Entities;

namespace Sereno.Documentation.DataAccess
{
    public class AppDbContext : DbContext
    {

        private readonly Context context;

        public AppDbContext(DbContextOptions<AppDbContext> options, Context context)
            : base(options)
        {
            this.context = context;
        }

        public static AppDbContext Create(string connectionString, Context context)
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlServer(connectionString)
                .Options;

            return new AppDbContext(options, context);
        }

        public DbSet<Document> Documents { get; set; }


        public override int SaveChanges()
        {
            LoggingUtility.SetSessionContext(context, Database.GetDbConnection());

            return base.SaveChanges();
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            await LoggingUtility.SetSessionContextAsync(context, Database.GetDbConnection());
            return await base.SaveChangesAsync(cancellationToken);
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            EntityFrameworkUtility.EnableTriggersOnTables(modelBuilder);
            EntityFrameworkUtility.SetDatabaseColumnPrefixes(modelBuilder);
        }
    }
}
