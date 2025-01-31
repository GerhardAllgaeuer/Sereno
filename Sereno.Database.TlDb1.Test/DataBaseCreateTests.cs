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
public sealed class DataBaseCreateTests
{
    private string connectionString = "";
    private Context appContext = ContextUtility.Create("autotest@test.com");

    [TestInitialize]
    public void Setup()
    {
        var configuration = ConfigurationUtility.GetConfiguration();
        connectionString = configuration.GetConnectionString("CreateTest_ConnectionString")!;
    }


    [TestMethod]
    public void Config_DatabaseCreate_Auto()
    {
        using var context = AppDbContext.Create(connectionString, appContext);

        // Datenbank neu erstellen
        context.Database.EnsureDeleted();
        context.Database.Migrate();

        // Log Datenbank neu erstellen
        LogDatabaseUtility.DeleteLogDatabase(connectionString);
        LogDatabaseUtility.UpdateLogDatabase(connectionString);
        TrackingUtility.CreateDefaultValuesTriggers(connectionString);


        // Log Datenbank ändern und erneut abgleichen
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
