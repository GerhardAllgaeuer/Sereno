using FluentAssertions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.Extensions.Configuration;
using Sereno.Database.ChangeTracking.TlDb1;
using Sereno.TlDb1.DataAccess;
using Sereno.TlDb1.DataAccess.Entities;
using FluentAssertions.DapperExtensions;
using Sereno.Utilities;

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
        LogDatabaseUtility.UpdateLogDatabase(connectionString);

        using var connection = new SqlConnection(logConnectionString);
        connection.Open();
        connection.Should().HaveColumnType("tstSimple", "vTitle", "nvarchar(500)");
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
