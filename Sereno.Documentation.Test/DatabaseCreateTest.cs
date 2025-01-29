using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Sereno.Database;
using Sereno.Documentation.DataAccess;
using Sereno.Documentation.DataAccess.Entities;
using Sereno.Utilities;

namespace Sereno.Documentation
{
    [TestClass]
    public sealed class DatabaseCreateTest
    {
        private string connectionString = "";


        [TestInitialize]
        public void Setup()
        {
            var configuration = ConfigurationHelper.GetConfiguration();
            connectionString = configuration.GetConnectionString("CreateTest_ConnectionString")!;
        }


        [TestMethod]
        public void Trigger_Create_Auto()
        {
            TriggerUtility.CreateDefaultValuesTriggers(connectionString);
        }


        [TestMethod]
        public void Config_DatabaseCreate_Auto()
        {
            using var context = AppDbContext.Create(connectionString, "inserter@test.com");


            // Datenbank neu erstellen
            context.Database.EnsureDeleted();
            context.Database.Migrate();

            // Log Datenbank neu erstellen
            LogDatabaseUtility.DeleteLogDatabase(connectionString);
            LogDatabaseUtility.UpdateLogDatabase(connectionString);


            // Log Datenbank ändern und erneut abgleichen
            ChangeLogDatabase();
            LogDatabaseUtility.UpdateLogDatabase(connectionString);


            List<Document> set = context.Documents.ToList();


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
                    Alter table docDocument alter column vTitle nvarchar(400);
                ";
                command.ExecuteNonQuery();
            }
        }
    }
}
