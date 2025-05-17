using FluentAssertions;
using Microsoft.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sereno.Test.Database;

namespace Sereno.Database.TlDb1.Test;

[TestClass]
public sealed class LogDatabaseTests : DatabaseTestBase
{


    [TestMethod]
    public void LogDatabaseExistance()
    {
        using var connection = new SqlConnection(this.LogConnectionString);
        connection.Open();
        connection.Should().HaveTable("tstSimple");
    }


    [TestMethod]
    public void LogDatabaseChange()
    {
        // Log Datenbank ändern und erneut abgleichen
        ChangeLogDatabase();
        LogDatabaseUtility.CreateOrUpdateLogDatabase(this.MasterConnectionString, connectionStringInfo.Database);

        using var connection = new SqlConnection(this.LogConnectionString);
        connection.Open();
        connection.Should().HaveColumnType("tstSimple", "vTitle", "nvarchar(500)");
    }


    private void ChangeLogDatabase()
    {
        using var command = this.LogConnection.CreateCommand();
        command.CommandText = $@"
                Alter table tstSimple alter column vTitle nvarchar(400);
            ";
        command.ExecuteNonQuery();
    }
}
