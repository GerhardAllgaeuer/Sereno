using Microsoft.EntityFrameworkCore;
using Sereno.Documentation.DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sereno.Documentation.DataAccess
{
    public class AppDbContext : DbContext
    {
        // Konstruktor
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<Document> Documents { get; set; }

        // Optionale Konfiguration
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Document>()
                .HasIndex(u => u.Id)
                .IsUnique();
        }
    }
}
