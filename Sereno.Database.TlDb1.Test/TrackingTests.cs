using Microsoft.Data.SqlClient;
using Sereno.Test.Database;
using Sereno.TlDb1.DataAccess;
using Sereno.TlDb1.DataAccess.Entities;


namespace Sereno.Database.TlDb1.Test;

[TestClass]
public sealed class TrackingTests : DatabaseTestBase
{
    [TestMethod]
    [TestProperty("Auto", "")]
    public void UpdateTracking()
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

        using var connection = new SqlConnection(connectionString);
        connection.Open();
        connection.DataRow("tstSimple", newDocument.Id).Column("vTitle").Be("Test Title");
    }
}
