using Microsoft.EntityFrameworkCore;
using Sereno.System.DataAccess.Entities;

namespace Sereno.System.DataAccess
{
    public class ConnexiaSeeder
    {
        public static void SeedMasterData(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>().HasData(
                State.All.Select(s => new
                {
                    s.Id,
                    s.Description
                })
            );
        }
    }
}
