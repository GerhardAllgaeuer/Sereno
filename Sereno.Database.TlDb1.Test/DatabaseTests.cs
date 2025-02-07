using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Sereno.TlDb1.DataAccess;
using Sereno.TlDb1.DataAccess.Entities;

namespace Sereno.Database.TlDb1.Test;

[TestClass]
public sealed class DatabaseTests : DatabaseTestBase
{
    [TestMethod]
    [TestProperty("Auto", "")]
    public void ReadSimpleTable()
    {
        using var context = AppDbContext.Create(connectionString, appContext);

        List<SimpleTable> set = context.SimpleTables.ToList();

        Assert.IsNotNull(context);
    }

    [TestMethod]
    [TestProperty("Auto", "")]
    public void InsertSimpleTable()
    {
        using var context = AppDbContext.Create(connectionString, appContext);

        var newDocument = new SimpleTable
        {
            Id = Guid.NewGuid().ToString(),
            Title = "Test Title",
            Description = "This is a test content",
        };

        context.SimpleTables.Add(newDocument);
        context.SaveChanges();

        var insertedDocument = context.SimpleTables.FirstOrDefault(d => d.Title == "Test Title");

        Assert.IsNotNull(insertedDocument, "Der Datensatz wurde nicht in der Datenbank gespeichert.");
        Assert.AreEqual("Test Title", insertedDocument.Title, "Der Titel des gespeicherten Dokuments ist falsch.");
        Assert.AreEqual("This is a test content", insertedDocument.Description, "Der Inhalt des gespeicherten Dokuments ist falsch.");
    }
}
