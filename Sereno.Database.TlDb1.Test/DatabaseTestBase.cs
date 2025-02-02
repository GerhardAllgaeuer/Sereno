using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Sereno.Database.ChangeTracking.TlDb1;
using Sereno.TlDb1.DataAccess;
using Sereno.Utilities;
using FluentAssertions;
using System;

namespace Sereno.Database.TlDb1.Test
{
    [TestClass]
    public class DatabaseTestBase
    {
        static bool createDatabase = false;

        protected string connectionString = "";
        protected string logConnectionString = "";
        protected Context appContext = ContextUtility.Create("autotest@test.com");

        [TestInitialize]
        public void SetupBase()
        {
            var configuration = ConfigurationUtility.GetConfiguration();
            connectionString = configuration.GetConnectionString("CreateTest_ConnectionString")!;
            logConnectionString = configuration.GetConnectionString("CreateTestLog_ConnectionString")!;
        }


        /// <summary>
        /// Einmalige Erstellung der Datenbank pro Test Durchlauf
        /// </summary>

        [AssemblyInitialize]
        public static void AssemblyInit(TestContext testContext)
        {
            if (createDatabase)
            {
                var configuration = ConfigurationUtility.GetConfiguration();
                string connectionString = configuration.GetConnectionString("CreateTest_ConnectionString")!;

                Context appContext = ContextUtility.Create("autotest@test.com");
                using var context = AppDbContext.Create(connectionString, appContext);

                // Datenbank neu erstellen
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();

                // Log Datenbank neu erstellen
                LogDatabaseUtility.DeleteLogDatabase(connectionString);
                LogDatabaseUtility.UpdateLogDatabase(connectionString);
                TrackingUtility.CreateDefaultValuesTriggers(connectionString);
            }
        }
    }
}
