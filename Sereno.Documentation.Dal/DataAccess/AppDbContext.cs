using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Sereno.Database;
using Sereno.Database.ChangeTracking;
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
            SetSessionContext();
            SetChangeTrackingData();

            return base.SaveChanges();
        }

        public void SetChangeTrackingData()
        {
            var entries = ChangeTracker.Entries()
                .Where(e => e.Entity is IChangeTracking &&
                (e.State == EntityState.Added || e.State == EntityState.Modified));

            foreach (var entry in entries)
            {
                var entity = (IChangeTracking)entry.Entity;

                if (entry.State == EntityState.Added)
                {
                    entity.Create = DateTime.Now;
                    entity.CreateUser = context.UserName;
                }

                entity.ModifyUser = context.UserName;
                entity.Modify = DateTime.Now;
            }
        }


        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            await SetSessionContextAsync();
            return await base.SaveChangesAsync(cancellationToken);
        }

        private void SetSessionContext()
        {
            if (Database.GetDbConnection().State == System.Data.ConnectionState.Closed)
            {
                Database.GetDbConnection().Open();
            }

            using var command = Database.GetDbConnection().CreateCommand();
            command.CommandText = "EXEC sp_set_session_context @key = @keyParam, @value = @valueParam";
            command.Parameters.Add(new SqlParameter("@keyParam", "UserName"));
            command.Parameters.Add(new SqlParameter("@valueParam", context.UserName));
            command.ExecuteNonQuery();
        }

        private async Task SetSessionContextAsync()
        {
            if (Database.GetDbConnection().State == System.Data.ConnectionState.Closed)
            {
                await Database.GetDbConnection().OpenAsync();
            }

            using var command = Database.GetDbConnection().CreateCommand();
            command.CommandText = "EXEC sp_set_session_context @key = @keyParam, @value = @valueParam";
            command.Parameters.Add(new SqlParameter("@keyParam", "UserName"));
            command.Parameters.Add(new SqlParameter("@valueParam", context.UserName));
            await command.ExecuteNonQueryAsync();
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Entities so konfigurieren, damit Trigger funktionieren
            EntityFrameworkUtility.EnableTriggersOnTables(modelBuilder);

            // Präfixe vor die Spalten setzen (z.B. vTitle, ...)
            EntityFrameworkUtility.SetDatabaseColumnPrefixes(modelBuilder);
        }



    }
}
