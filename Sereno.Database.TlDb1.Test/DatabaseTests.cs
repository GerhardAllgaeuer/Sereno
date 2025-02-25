using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Sereno.Test.Database;
using Sereno.TlDb1.DataAccess;
using Sereno.TlDb1.DataAccess.Entities;

namespace Sereno.Database.TlDb1.Test;

[TestClass]
public sealed class DatabaseTests : DatabaseTestBase
{
    [TestMethod]
    public void Read_From_SimpleTable()
    {
        using var dbContext = TestDbContextFactory.Create(appContext);

        List<SimpleTable> set = dbContext.SimpleTables.ToList();

        Assert.IsNotNull(dbContext);
    }


    [TestMethod]
    [DoNotParallelize]
    public void Insert_Into_SimpleTable()
    {
        // Tabelle bereinigen
        DatabaseUtility.TruncateTables(this.Connection, ["tstSimple"]);

        // EF Inserts
        using var dbContext = TestDbContextFactory.Create(appContext);

        var newEntry1 = new SimpleTable
        {
            Id = Guid.NewGuid().ToString(),
            Title = "Title 1",
            Description = "Description 1",
        };

        dbContext.SimpleTables.Add(newEntry1);
        dbContext.SaveChanges();


        var newEntry2 = new SimpleTable
        {
            Id = Guid.NewGuid().ToString(),
            Title = "Title 2",
            Description = "Description 2",
        };

        dbContext.SimpleTables.Add(newEntry2);
        dbContext.SaveChanges();


        // Zeilen prüfen
        this.Connection.DataRows("tstSimple", null, "dModify")
            .Should().ContainValues(
            [
                new { vTitle = "Title 1", vDescription = "Description 1" },
                new { vTitle = "Title 2", vDescription = "Description 2" },
            ]);

    }
}
