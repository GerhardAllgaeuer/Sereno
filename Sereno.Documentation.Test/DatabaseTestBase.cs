using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Sereno.Database;
using Sereno.Database.ChangeTracking.TlDb1;
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
            connectionString = configuration.GetConnectionString("Documentation_ConnectionString")!;
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
                string connectionString = configuration.GetConnectionString("Documentation_ConnectionString")!;
                string masterConnectionString = ConnectionStringUtility.ChangeDatabaseName(connectionString, "master");
                ConnectionStringInfo connectionStringInfo = ConnectionStringUtility.ParseConnectionString(connectionString);

                // Datenbanken löschen
                LogDatabaseUtility.DropDatabaseAndLogDatabase(masterConnectionString, connectionStringInfo.Database);

                // Datenbank erstellen
                Context appContext = ContextUtility.Create("autotest@test.com");
                using var context = AppDbContext.Create(connectionString, appContext);
                context.Database.EnsureCreated();

                // Log Datenbank erstellen
                TrackingUtility.EnableTrackingAndCreateLogDatabase(masterConnectionString, connectionStringInfo.Database);
            }
        }
    }
}
