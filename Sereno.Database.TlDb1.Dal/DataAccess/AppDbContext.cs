using Microsoft.EntityFrameworkCore;
using Sereno.Database;
using Sereno.Database.Logging.TlDb1;
using Sereno.TlDb1.DataAccess.Entities;

namespace Sereno.TlDb1.DataAccess
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

        public DbSet<SimpleTable> SimpleTables { get; set; }


        public override int SaveChanges()
        {
            LoggingUtility.SetSessionContext(context, Database.GetDbConnection());
            SetChangeTrackingData();

            return base.SaveChanges();
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            await LoggingUtility.SetSessionContextAsync(context, Database.GetDbConnection());
            return await base.SaveChangesAsync(cancellationToken);
        }




        public void SetChangeTrackingData()
        {
            var entries = ChangeTracker.Entries()
                .Where(e => e.Entity is ILogging &&
                (e.State == EntityState.Added || e.State == EntityState.Modified));

            foreach (var entry in entries)
            {
                //var entity = (IChangeTracking)entry.Entity;

                //if (entry.State == EntityState.Added)
                //{
                //    entity.Create = DateTime.Now;
                //    entity.CreateUser = context.UserName;
                //}

                //entity.ModifyUser = context.UserName;
                //entity.Modify = DateTime.Now;
            }
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            EntityFrameworkUtility.EnableTriggersOnTables(modelBuilder);
            EntityFrameworkUtility.SetDatabaseColumnPrefixes(modelBuilder);
        }



    }
}
