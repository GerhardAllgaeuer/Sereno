using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Sereno.Database.ChangeTracking.TlDb1;
using Sereno.Test.Database;
using Sereno.TlDb1.DataAccess;
using Sereno.Utilities;
using System.Diagnostics.CodeAnalysis;
using System.Xml.Linq;

namespace Sereno.Database.TlDb1.Test
{
    [TestClass]
    public abstract class DatabaseTestBase
    {
        static bool createDatabase = true;

        protected SqlConnection connection = new();
        protected string connectionString = "";
        protected ConnectionStringInfo? connectionStringInfo = null;

        protected SqlConnection logConnection = new();
        protected string logConnectionString = "";

        protected SqlConnection masterConnection = new();
        protected string masterConnectionString = "";

        protected Context appContext = ContextUtility.Create("autotest@test.com");


        [TestInitialize]
        public void SetupBase()
        {
            var configuration = ConfigurationUtility.GetConfiguration();
            connectionString = configuration.GetConnectionString("CreateTest_ConnectionString")!;
            connection = new SqlConnection(connectionString);
            connection.Open();

            connectionStringInfo = ConnectionStringUtility.ParseConnectionString(connectionString);

            string logDatabaseName = LogDatabaseUtility.GetLogDatabaseName(connectionStringInfo.Database);
            logConnectionString = ConnectionStringUtility.ChangeDatabaseName(connectionString, logDatabaseName);
            logConnection = new SqlConnection(logConnectionString);
            logConnection.Open();

            masterConnectionString = ConnectionStringUtility.ChangeDatabaseName(connectionString, "master");
            masterConnection = new SqlConnection(masterConnectionString);
            masterConnection.Open();
        }



        [TestCleanup]
        public void Cleanup()
        {
            connection?.Dispose(); // Verbindung sauber schließen
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
