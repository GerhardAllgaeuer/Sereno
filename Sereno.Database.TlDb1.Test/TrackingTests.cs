using Microsoft.Data.SqlClient;
using Sereno.Test.Database;
using Sereno.TlDb1.DataAccess;
using Sereno.TlDb1.DataAccess.Entities;


namespace Sereno.Database.TlDb1.Test;

[TestClass]
public sealed class TrackingTests : DatabaseTestBase
{
    [TestMethod]
    [DoNotParallelize]
    [TestProperty("Auto", "")]
    public void UpdateTracking()
    {
        // Tabelle bereinigen
        DatabaseUtility.TruncateTables(connection, "tstSimple");


        // EF Inserts
        using var context = AppDbContext.Create(connectionString, appContext);

        var newDocument1 = new SimpleTable
        {
            Id = Guid.NewGuid().ToString(),
            Title = "Title 1",
            Description = "Description 1",
        };

        context.SimpleTables.Add(newDocument1);
        context.SaveChanges();


        var newDocument2 = new SimpleTable
        {
            Id = Guid.NewGuid().ToString(),
            Title = "Title 2",
            Description = "Description 2",
        };

        context.SimpleTables.Add(newDocument2);
        context.SaveChanges();



        // Werte prüfen
        connection!.DataRow("tstSimple", newDocument1.Id).Column("vTitle").Should().Be("Title 1");

        connection!.DataRows("tstSimple", null, "dModify")
            .Should().ContainValues(
            [
                new { vTitle = "Title 1", vDescription = "Description 1" },
                new { vTitle = "Title 2", vDescription = "Description 2" },
            ]);

    }
}
