using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.Extensions.Configuration;
using Sereno.Database.ChangeTracking.TlDb1;
using Sereno.TlDb1.DataAccess;
using Sereno.TlDb1.DataAccess.Entities;
using Sereno.Utilities;

namespace Sereno.Database.TlDb1.Test;

[TestClass]
public sealed class DatabaseCreateTests : DatabaseTestBase
{

    [TestMethod]
    public void Test_Create_Init()
    {
    }



    [TestMethod]
    public void Config_LogDatabase_Update()
    {
        using var context = AppDbContext.Create(connectionString, appContext);

        // Log Datenbank �ndern und erneut abgleichen
        ChangeLogDatabase();
        LogDatabaseUtility.UpdateLogDatabase(connectionString);


        List<SimpleTable> set = context.Documents.ToList();


        Assert.IsNotNull(context);
    }

    private void ChangeLogDatabase()
    {
        string logConnectionString = LogDatabaseUtility.GetLogDatabaseConnectionString(connectionString);
        using (var connection = new SqlConnection(logConnectionString))
        {
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = $@"
                    Alter table tstSimple alter column vTitle nvarchar(400);
                ";
            command.ExecuteNonQuery();
        }
    }
}
