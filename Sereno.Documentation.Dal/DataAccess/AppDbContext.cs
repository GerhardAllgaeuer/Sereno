using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Sereno.Database;
using Sereno.Documentation.DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Sereno.Documentation.DataAccess
{
    public class AppDbContext : DbContext
    {

        private readonly string userName;

        public AppDbContext(DbContextOptions<AppDbContext> options, string userName)
            : base(options)
        {
            this.userName = userName;
        }

        public static AppDbContext Create(string connectionString, string userName)
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlServer(connectionString)
                .Options;

            return new AppDbContext(options, userName);
        }

        public DbSet<Document> Documents { get; set; }

        public override int SaveChanges()
        {
            SetSessionContext();
            return base.SaveChanges();
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
            command.Parameters.Add(new SqlParameter("@valueParam", this.userName));
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
            command.Parameters.Add(new SqlParameter("@valueParam", this.userName));
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
