using Sereno.Test.Database;
using Sereno.TlDb1.DataAccess;
using Sereno.TlDb1.DataAccess.Entities;
using FluentAssertions;
using Sereno.Utilities;

namespace Sereno.Database.TlDb1.Test;

[TestClass]
public sealed class LogTests : DatabaseTestBase
{
    [TestMethod]
    [DoNotParallelize]
    [TestProperty("Auto", "")]
    public void Set_Create_And_Modify_Correctly()
    {
        DatabaseUtility.TruncateTables(connection, ["tstSimple"]);
        DatabaseUtility.TruncateTables(logConnection, ["tstSimple", "logChange"]);

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
            entryFound!.Description = "Description 2";
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



    [TestMethod]
    [DoNotParallelize]
    [TestProperty("Auto", "")]
    public void Log_Insert_Update_Delete_At_LogDatabase_Correctly()
    {
        DatabaseUtility.TruncateTables(connection, ["tstSimple"]);
        DatabaseUtility.TruncateTables(logConnection, ["tstSimple", "logChange"]);

        // Insert
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

        // Log Table Eintrag muss vorhanden sein
        logConnection.DataRows("tstSimple", null, "tTimestamp")
            .Should().ContainValues(
            [
                new { vChangeType = "I", vUserName = "autotest@test.com", vId = newEntry.Id, vTitle = "Title 1", vDescription = "Description 1" },
            ]);

        // Log History Eintrag muss vorhanden sein
        logConnection.DataRows("logChange", null, "tTimestamp")
            .Should().ContainValues(
            [
                new { vChangeType = "I", vPrimaryKey = newEntry.Id, vTable = "tstSimple", vUserName = "autotest@test.com" },
            ]);





        // Update mit anderem User
        Context appContext2 = ContextUtility.Create("autotest2@test.com");
        using (var context = AppDbContext.Create(connectionString, appContext2))
        {
            SimpleTable? entryFound = context.SimpleTables.Find(newEntry.Id);
            entryFound!.Title = "Title 2";
            entryFound!.Description = "Description 2";
            context.SaveChanges();
        }


        // Log Table Eintrag muss vorhanden sein
        logConnection.DataRows("tstSimple", "vChangeType in ('U', 'UO')", "tTimestamp")
            .Should().ContainValues(
            [
                new { vChangeType = "UO", vUserName = "autotest2@test.com", vId = newEntry.Id, vTitle = "Title 1", vDescription = "Description 1", vModifyUser = "autotest@test.com" },
                new { vChangeType = "U", vUserName = "autotest2@test.com", vId = newEntry.Id, vTitle = "Title 2", vDescription = "Description 2", vModifyUser = "autotest2@test.com" },
            ]);

        // Log History Eintrag muss vorhanden sein
        logConnection.DataRows("logChange", "vChangeType in ('U')", "tTimestamp")
            .Should().ContainValues(
            [
                new { vChangeType = "U", vPrimaryKey = newEntry.Id, vTable = "tstSimple", vUserName = "autotest2@test.com" },
            ]);





        // Delete 
        using (var context = AppDbContext.Create(connectionString, appContext2))
        {
            SimpleTable? entryFound = context.SimpleTables.Find(newEntry.Id);
            if (entryFound != null)
            {
                context.SimpleTables.Remove(entryFound);
                context.SaveChanges();
            }
        }


        // Log Table Eintrag muss vorhanden sein
        logConnection.DataRows("tstSimple", "vChangeType in ('D')", "tTimestamp")
            .Should().ContainValues(
            [
                new { vChangeType = "D", vUserName = "autotest2@test.com", vId = newEntry.Id, vTitle = "Title 2", vDescription = "Description 2", vModifyUser = "autotest2@test.com" },
            ]);

        // Log History Eintrag muss vorhanden sein
        logConnection.DataRows("logChange", "vChangeType in ('D')", "tTimestamp")
            .Should().ContainValues(
            [
                new { vChangeType = "D", vPrimaryKey = newEntry.Id, vTable = "tstSimple", vUserName = "autotest2@test.com" },
            ]);


    }
}
