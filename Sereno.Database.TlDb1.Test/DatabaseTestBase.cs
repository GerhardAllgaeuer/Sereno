using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Sereno.Database.Logging.TlDb1;
using Sereno.Utilities;

namespace Sereno.Database.TlDb1.Test
{
    [TestClass]
    public abstract class DatabaseTestBase
    {
        static bool createDatabase = true;

        protected SqlConnection Connection { get; private set; } = new();
        protected string ConnectionString { get; private set; } = "";
        protected ConnectionStringInfo connectionStringInfo = new();

        protected SqlConnection LogConnection { get; private set; } = new();
        protected string LogConnectionString { get; private set; } = "";

        protected SqlConnection MasterConnection { get; private set; } = new();
        protected string MasterConnectionString { get; private set; } = "";

        protected Context appContext = ContextUtility.Create("autotest@test.com");


        [TestInitialize]
        public void SetupBase()
        {
            var configuration = ConfigurationUtility.GetConfiguration();
            ConnectionString = configuration.GetConnectionString("TestDb_ConnectionString")!;
            Connection = new SqlConnection(ConnectionString);
            Connection.Open();

            connectionStringInfo = ConnectionStringUtility.ParseConnectionString(ConnectionString);

            string logDatabaseName = LogDatabaseUtility.GetLogDatabaseName(connectionStringInfo.Database);
            this.LogConnectionString = ConnectionStringUtility.ChangeDatabaseName(ConnectionString, logDatabaseName);
            this.LogConnection = new SqlConnection(this.LogConnectionString);
            this.LogConnection.Open();

            this.MasterConnectionString = ConnectionStringUtility.ChangeDatabaseName(ConnectionString, "master");
            this.MasterConnection = new SqlConnection(this.MasterConnectionString);
            this.MasterConnection.Open();
        }



        [TestCleanup]
        public void Cleanup()
        {
            this.Connection?.Dispose();
            this.LogConnection?.Dispose();
            this.MasterConnection?.Dispose();
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
