using Sereno.Test.Database;
using Sereno.TlDb1.DataAccess;
using Sereno.TlDb1.DataAccess.Entities;
using FluentAssertions;
using Sereno.Utilities;

namespace Sereno.Database.TlDb1.Test;

[TestClass]
public sealed class TrackingTests : DatabaseTestBase
{
    [TestMethod]
    [DoNotParallelize]
    [TestProperty("Auto", "")]
    public void Track_Insert_And_Update_Correctly()
    {
        DatabaseUtility.TruncateTables(connection, "tstSimple");
        DatabaseUtility.TruncateTables(logConnection, "tstSimple");

        // Vergleichszeit zum Testen
        DateTime insertTime = DateTime.Now;

        var newEntry = new SimpleTable
        {
            Id = Guid.NewGuid().ToString(),
            Title = "Title 1",
            Description = "Description 1",
        };

        using (var context = AppDbContext.Create(connectionString, appContext))
        {
            context.SimpleTables.Add(newEntry);
            context.SaveChanges();
        }

        // Auslesen der Trigger geänderten Werte
        DateTime create = connection.DataRow("tstSimple", newEntry.Id)
            .Column<DateTime>("dCreate");

        DateTime modify = connection.DataRow("tstSimple", newEntry.Id)
            .Column<DateTime>("dModify");


        // Werte müssen ungefähr dem Insert Zeitpunkt entsprechen
        modify.Should().BeCloseTo(insertTime, TimeSpan.FromSeconds(10));
        create.Should().BeCloseTo(insertTime, TimeSpan.FromSeconds(10));

        // Werte müssen gleich sein, weil der Trigger ja die gleich Zeit einfügt
        modify.Should().Be(create);



        object createUser = connection.DataRow("tstSimple", newEntry.Id)
            .Column("vCreateUser");

        object modifyUser = connection.DataRow("tstSimple", newEntry.Id)
            .Column("vModifyUser");

        // Benutzer, muss der sein, der über den App Context gesetzt wurde
        createUser.Should().Be(appContext.UserName);
        modifyUser.Should().Be(appContext.UserName);




        // Update mit anderem User
        Context appContext2 = ContextUtility.Create("autotest2@test.com");
        using (var context = AppDbContext.Create(connectionString, appContext2))
        {
            SimpleTable? entryFound = context.SimpleTables.Find(newEntry.Id);
            entryFound!.Title = "Title 2";
            context.SaveChanges();
        }

        // Auslesen der Trigger geänderten Werte
        create = connection.DataRow("tstSimple", newEntry.Id)
            .Column<DateTime>("dCreate");

        modify = connection.DataRow("tstSimple", newEntry.Id)
            .Column<DateTime>("dModify");

        createUser = connection.DataRow("tstSimple", newEntry.Id)
            .Column("vCreateUser");

        modifyUser = connection.DataRow("tstSimple", newEntry.Id)
            .Column("vModifyUser");

        // Create Werte sollen unverändert sein
        create.Should().BeCloseTo(insertTime, TimeSpan.FromSeconds(10));
        createUser.Should().Be(appContext.UserName);

        // Geändertes Datum muss höher, als das Erstelldatum sein
        modify.Should().BeAfter(create);
        modifyUser.Should().Be(appContext2.UserName);

    }
}
