using Sereno.Test.Database;
using Sereno.TlDb1.DataAccess;
using Sereno.TlDb1.DataAccess.Entities;
using FluentAssertions;
using Sereno.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Sereno.Database.TlDb1.Test;

[TestClass]
public sealed class LogTests : DatabaseTestBase
{
    [TestMethod]
    [DoNotParallelize]
    public void Set_Create_And_Modify_Correctly()
    {
        DatabaseUtility.TruncateTables(this.Connection, ["tstSimple"]);
        DatabaseUtility.TruncateTables(this.LogConnection, ["tstSimple", "logChange"]);

        // Vergleichszeit zum Testen
        DateTime insertTime = DateTime.Now;

        var newEntry = new SimpleTable
        {
            Id = Guid.NewGuid().ToString(),
            Title = "Title 1",
            Description = "Description 1",
        };

        using (var dbContext = TestDbContextFactory.Create(appContext))
        {
            dbContext.SimpleTables.Add(newEntry);
            dbContext.SaveChanges();
        }

        // Auslesen der Trigger geänderten Werte
        DateTime create = this.Connection.DataRow("tstSimple", newEntry.Id)
            .Column<DateTime>("dCreate");

        DateTime modify = this.Connection.DataRow("tstSimple", newEntry.Id)
            .Column<DateTime>("dModify");


        // Werte müssen ungefähr dem Insert Zeitpunkt entsprechen
        modify.Should().BeCloseTo(insertTime, TimeSpan.FromSeconds(10));
        create.Should().BeCloseTo(insertTime, TimeSpan.FromSeconds(10));

        // Werte müssen gleich sein, weil der Trigger ja die gleich Zeit einfügt
        modify.Should().Be(create);



        object createUser = this.Connection.DataRow("tstSimple", newEntry.Id)
            .Column("vCreateUser");

        object modifyUser = this.Connection.DataRow("tstSimple", newEntry.Id)
            .Column("vModifyUser");

        // Benutzer, muss der sein, der über den App Context gesetzt wurde
        createUser.Should().Be(appContext.UserName);
        modifyUser.Should().Be(appContext.UserName);





        // Update mit anderem User
        Context appContext2 = ContextUtility.Create("autotest2@test.com");
        using (var dbContext = TestDbContextFactory.Create(appContext2))
        {
            SimpleTable? entryFound = dbContext.SimpleTables.Find(newEntry.Id);
            entryFound!.Title = "Title 2";
            entryFound!.Description = "Description 2";
            dbContext.SaveChanges();
        }

        // Auslesen der Trigger geänderten Werte
        create = this.Connection.DataRow("tstSimple", newEntry.Id)
            .Column<DateTime>("dCreate");

        modify = this.Connection.DataRow("tstSimple", newEntry.Id)
            .Column<DateTime>("dModify");

        createUser = this.Connection.DataRow("tstSimple", newEntry.Id)
            .Column("vCreateUser");

        modifyUser = this.Connection.DataRow("tstSimple", newEntry.Id)
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
    public void Log_Insert_Update_Delete_At_LogDatabase_Correctly()
    {
        DatabaseUtility.TruncateTables(this.Connection, ["tstSimple"]);
        DatabaseUtility.TruncateTables(this.LogConnection, ["tstSimple", "logChange"]);

        // Insert
        var newEntry = new SimpleTable
        {
            Id = Guid.NewGuid().ToString(),
            Title = "Title 1",
            Description = "Description 1",
        };

        using (var dbContext = TestDbContextFactory.Create(appContext))
        {
            dbContext.SimpleTables.Add(newEntry);
            dbContext.SaveChanges();
        }

        // Log Table Eintrag muss vorhanden sein
        this.LogConnection.DataRows("Select * from tstSimple order by tTimestamp")
            .Should().ContainValues(new object[] 
            {
                new { vChangeType = "I", vUserName = "autotest@test.com", vId = newEntry.Id, vTitle = "Title 1", vDescription = "Description 1" },
            });

        // Log Table Eintrag muss vorhanden sein
        this.LogConnection.DataRows("tstSimple", null, "tTimestamp")
            .Should().ContainValues(new object[]
            {
                new { vChangeType = "I", vUserName = "autotest@test.com", vId = newEntry.Id, vTitle = "Title 1", vDescription = "Description 1" },
            });

        // Log History Eintrag muss vorhanden sein
        this.LogConnection.DataRows("logChange", null, "tTimestamp")
            .Should().ContainValues(new object[]
            {
                new { vChangeType = "I", vPrimaryKey = newEntry.Id, vTable = "tstSimple", vUserName = "autotest@test.com" },
            });





        // Update mit anderem User
        Context appContext2 = ContextUtility.Create("autotest2@test.com");
        using (var dbContext = TestDbContextFactory.Create(appContext2))
        {
            SimpleTable? entryFound = dbContext.SimpleTables.Find(newEntry.Id);
            entryFound!.Title = "Title 2";
            entryFound!.Description = "Description 2";
            dbContext.SaveChanges();
        }


        // Log Table Eintrag muss vorhanden sein
        this.LogConnection.DataRows("tstSimple", "vChangeType in ('U', 'UO')", "tTimestamp")
            .Should().ContainValues(new object[]
            {
                new { vChangeType = "UO", vUserName = "autotest2@test.com", vId = newEntry.Id, vTitle = "Title 1", vDescription = "Description 1", vModifyUser = "autotest@test.com" },
                new { vChangeType = "U", vUserName = "autotest2@test.com", vId = newEntry.Id, vTitle = "Title 2", vDescription = "Description 2", vModifyUser = "autotest2@test.com" },
            });

        // Log History Eintrag muss vorhanden sein
        this.LogConnection.DataRows("logChange", "vChangeType in ('U')", "tTimestamp")
            .Should().ContainValues(new object[]
            {
                new { vChangeType = "U", vPrimaryKey = newEntry.Id, vTable = "tstSimple", vUserName = "autotest2@test.com" },
            });





        // Delete 
        using (var dbContext = TestDbContextFactory.Create(appContext2))
        {
            SimpleTable? entryFound = dbContext.SimpleTables.Find(newEntry.Id);
            if (entryFound != null)
            {
                dbContext.SimpleTables.Remove(entryFound);
                dbContext.SaveChanges();
            }
        }


        // Log Table Eintrag muss vorhanden sein
        this.LogConnection.DataRows("tstSimple", "vChangeType in ('D')", "tTimestamp")
            .Should().ContainValues(new object[]
            {
                new { vChangeType = "D", vUserName = "autotest2@test.com", vId = newEntry.Id, vTitle = "Title 2", vDescription = "Description 2", vModifyUser = "autotest2@test.com" },
            });

        // Log History Eintrag muss vorhanden sein
        this.LogConnection.DataRows("logChange", "vChangeType in ('D')", "tTimestamp")
            .Should().ContainValues(new object[]
            {
                new { vChangeType = "D", vPrimaryKey = newEntry.Id, vTable = "tstSimple", vUserName = "autotest2@test.com" },
            });


    }
}
