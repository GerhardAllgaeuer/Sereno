﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Sereno.Database;
using Sereno.Database.Logging.TlDb1;
using Sereno.Documentation.DataAccess;
using Sereno.Utilities;

namespace Sereno.Documentation.Test
{
    [TestClass]
    public class DatabaseTestBase
    {
        static bool createDatabase = true;

        protected string connectionString = "";
        protected string logConnectionString = "";
        protected Context appContext = ContextUtility.Create("autotest@test.com");

        [TestInitialize]
        public void SetupBase()
        {
            var configuration = ConfigurationUtility.GetConfiguration();
            connectionString = configuration.GetConnectionString("TestDb_ConnectionString")!;
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
                string connectionString = configuration.GetConnectionString("TestDb_ConnectionString")!;
                string masterConnectionString = ConnectionStringUtility.ChangeDatabaseName(connectionString, "master");
                ConnectionStringInfo connectionStringInfo = ConnectionStringUtility.ParseConnectionString(connectionString);

                // Datenbanken löschen
                LogDatabaseUtility.DropDatabaseWithLogDatabase(masterConnectionString, connectionStringInfo.Database);

                // Datenbank erstellen
                Context appContext = ContextUtility.Create("autotest@test.com");
                using var context = TestDbContextFactory.Create(appContext);
                context.Database.EnsureCreated();

                // Log Datenbank erstellen
                LoggingUtility.EnableLogging(masterConnectionString, connectionStringInfo.Database);
            }
        }
    }
}
