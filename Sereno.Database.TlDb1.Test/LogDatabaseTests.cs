using FluentAssertions;
using Microsoft.Data.SqlClient;
using Sereno.Test.Database;

namespace Sereno.Database.TlDb1.Test;

[TestClass]
public sealed class LogDatabaseTests : DatabaseTestBase
{


    [TestMethod]
    [TestProperty("Auto", "")]
    public void LogDatabaseExistance()
    {
        using var connection = new SqlConnection(logConnectionString);
        connection.Open();
        connection.Should().HaveTable("tstSimple");
    }


    [TestMethod]
    [TestProperty("Auto", "")]
    public void LogDatabaseChange()
    {
        // Log Datenbank ändern und erneut abgleichen
        ChangeLogDatabase();
        LogDatabaseUtility.UpdateLogDatabase(masterConnectionString, connectionStringInfo.Database);

        using var connection = new SqlConnection(logConnectionString);
        connection.Open();
        connection.Should().HaveColumnType("tstSimple", "vTitle", "nvarchar(500)");
    }


    private void ChangeLogDatabase()
    {
        using var command = logConnection.CreateCommand();
        command.CommandText = $@"
                Alter table tstSimple alter column vTitle nvarchar(400);
            ";
        command.ExecuteNonQuery();
    }
}
