using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Sereno.Database;
using Sereno.Database.Logging.TlDb1;
using Sereno.Utilities;

namespace Sereno.Documentation.Test
{
    [TestClass]
    public class DocumentationTestBase
    {
        static bool createTestDatabase = true;
        static bool createDevelopmentDatabase = false;

        protected string connectionString = "";
        protected string logConnectionString = "";
        protected Context appContext = ContextUtility.Create("autotest@test.com");

        [TestInitialize]
        public void SetupBase()
        {
            var configuration = ConfigurationUtility.GetConfiguration();
            connectionString = configuration.GetConnectionString("TestDb_ConnectionString")!;
        }


        protected DocumentationLibrary CreateDocumentationLibrary()
        {
            var configuration = ConfigurationUtility.GetConfiguration();

            DocumentationLibrary result = new DocumentationLibrary()
            {
                DatabaseConnectionString = configuration.GetConnectionString("Development_ConnectionString")!,
                SourceRootDirectory = new DirectoryInfo(@"D:\Data\Dokumentation"),
                TargetFilesDirectory = new DirectoryInfo(@"D:\Projekte\Privat\Sereno\Sereno.Documentation.Client\public\images"),
            };

            return result;
        }


        protected DocumentationLibrary CreateTestLibrary()
        {
            var configuration = ConfigurationUtility.GetConfiguration();

            DocumentationLibrary result = new DocumentationLibrary()
            {
                DatabaseConnectionString = configuration.GetConnectionString("TestDb_ConnectionString")!,
                SourceRootDirectory = new DirectoryInfo($@"{CodeUtility.GetProjectRoot()}\Sereno.Documentation.Test\DocumentsLibrary"),
                TargetFilesDirectory = new DirectoryInfo($@"{CodeUtility.GetDataDirectory()}\Sereno.Office\"),
            };

            return result;
        }

        /// <summary>
        /// Einmalige Erstellung der Datenbank pro Test Durchlauf
        /// </summary>

        [AssemblyInitialize]
        public static void AssemblyInit(TestContext testContext)
        {
            var configuration = ConfigurationUtility.GetConfiguration();

            if (createTestDatabase)
            {
                string connectionString = configuration.GetConnectionString("TestDb_ConnectionString")!;
                CreateDatabase(connectionString);
            }

            if (createDevelopmentDatabase)
            {
                // Development DB
                string connectionString = configuration.GetConnectionString("Development_ConnectionString")!;
                CreateDatabase(connectionString);
            }

        }


        private static void CreateDatabase(string connectionString)
        {
            var configuration = ConfigurationUtility.GetConfiguration();
            string masterConnectionString = ConnectionStringUtility.ChangeDatabaseName(connectionString, "master");
            ConnectionStringInfo connectionStringInfo = ConnectionStringUtility.ParseConnectionString(connectionString);

            // Datenbanken löschen
            LogDatabaseUtility.DropDatabaseWithLogDatabase(masterConnectionString, connectionStringInfo.Database);

            // Datenbank erstellen
            Context appContext = ContextUtility.Create("autotest@test.com");
            using var context = DbContextFactory.Create(appContext, connectionString);
            context.Database.EnsureCreated();

            // Log Datenbank erstellen
            LoggingUtility.EnableLogging(masterConnectionString, connectionStringInfo.Database);
        }
    }
}
