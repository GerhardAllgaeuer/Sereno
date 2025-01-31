using Microsoft.EntityFrameworkCore;
using Sereno.Documentation.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sereno.Documentation
{
    public static class DbContextFactory
    {
        //public static AppDbContext CreateIntegrationDbContext()
        //{
        //    var options = new DbContextOptionsBuilder<AppDbContext>()
        //        .UseSqlServer("DeinTestConnectionString")
        //    .Options;

        //    var context = new AppDbContext(options);
        //    context.Database.EnsureCreated();
        //    return context;
        //}

        //public static AppDbContext CreateUnitTestDbContext()
        //{
        //    var options = new DbContextOptionsBuilder<AppDbContext>()
        //        .UseInMemoryDatabase("UnitTestDb")
        //    .Options;

        //    return new AppDbContext(options);
        //}
    }
}
