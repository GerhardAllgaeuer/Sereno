using Sereno.Test.Database;
using Sereno.TlDb1.DataAccess;
using Sereno.TlDb1.DataAccess.Entities;
using FluentAssertions.Extensions;
using FluentAssertions;

namespace Sereno.Database.TlDb1.Test;

[TestClass]
public sealed class TrackingTests : DatabaseTestBase
{
    [TestMethod]
    [DoNotParallelize]
    [TestProperty("Auto", "")]
    public void InsertTracking()
    {
        DatabaseUtility.TruncateTables(connection, "tstSimple");

        using var context = AppDbContext.Create(connectionString, appContext);

        DateTime insertTime = DateTime.Now;

        var newEntry = new SimpleTable
        {
            Id = Guid.NewGuid().ToString(),
            Title = "Title 1",
            Description = "Description 1",
        };

        context.SimpleTables.Add(newEntry);
        context.SaveChanges();


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

    }
}
