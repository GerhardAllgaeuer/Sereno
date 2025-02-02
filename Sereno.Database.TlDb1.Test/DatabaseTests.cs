using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Sereno.TlDb1.DataAccess;
using Sereno.TlDb1.DataAccess.Entities;

namespace Sereno.Database.TlDb1.Test;

[TestClass]
public sealed class DatabaseTests : DatabaseTestBase
{
    [TestMethod]
    [TestProperty("Auto", "")]
    public void ReadSimpleTable_Auto()
    {
        using var context = AppDbContext.Create(connectionString, appContext);

        List<SimpleTable> set = context.Documents.ToList();

        Assert.IsNotNull(context);
    }
}
