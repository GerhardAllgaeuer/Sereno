using Microsoft.EntityFrameworkCore;
using Sereno.System.DataAccess.Entities;

namespace Sereno.System.DataAccess
{
    public class Seeder
    {
        public static void SeedMasterData(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<State>().HasData(
                State.All.Select(s => new
                {
                    s.Id,
                    s.Description
                })
            );
        }
    }
}
