using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Sereno.Test.Database;
using Sereno.TlDb1.DataAccess;
using Sereno.TlDb1.DataAccess.Entities;

namespace Sereno.Database.TlDb1.Test;

[TestClass]
public sealed class DatabaseTests : DatabaseTestBase
{
    [TestMethod]
    [TestProperty("Auto", "")]
    public void Read_Records_From_SimpleTable()
    {
        using var context = AppDbContext.Create(connectionString, appContext);

        List<SimpleTable> set = context.SimpleTables.ToList();

        Assert.IsNotNull(context);
    }


    [TestMethod]
    [DoNotParallelize]
    [TestProperty("Auto", "")]
    public void Inserting_Into_SimpleTable()
    {
        // Tabelle bereinigen
        DatabaseUtility.TruncateTables(connection, ["tstSimple"]);

        // EF Inserts
        using var context = AppDbContext.Create(connectionString, appContext);

        var newEntry1 = new SimpleTable
        {
            Id = Guid.NewGuid().ToString(),
            Title = "Title 1",
            Description = "Description 1",
        };

        context.SimpleTables.Add(newEntry1);
        context.SaveChanges();


        var newEntry2 = new SimpleTable
        {
            Id = Guid.NewGuid().ToString(),
            Title = "Title 2",
            Description = "Description 2",
        };

        context.SimpleTables.Add(newEntry2);
        context.SaveChanges();


        // Zeilen prüfen
        connection.DataRows("tstSimple", null, "dModify")
            .Should().ContainValues(
            [
                new { vTitle = "Title 1", vDescription = "Description 1" },
                new { vTitle = "Title 2", vDescription = "Description 2" },
            ]);

    }
}
