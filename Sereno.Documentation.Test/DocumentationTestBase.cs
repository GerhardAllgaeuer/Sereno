using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Testing.Platform.Configurations;
using Sereno.Database;
using Sereno.Database.Logging.TlDb1;
using Sereno.Documentation.DataAccess;
using Sereno.Utilities;

namespace Sereno.Documentation.Test
{
    [TestClass]
    public class DocumentationTestBase
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


        protected DocumentationLibrary CreateDocumentationLibrary()
        {
            var configuration = ConfigurationUtility.GetConfiguration();

            DocumentationLibrary result = new DocumentationLibrary()
            {
                DatabaseConnectionString = configuration.GetConnectionString("Development_ConnectionString")!,
                DocumentsDirectory = new DirectoryInfo(@"D:\Data\Dokumentation"),
                HtmlExportDirectory = new DirectoryInfo(@"D:\Projekte\Privat\Sereno\Sereno.Documentation.Client\images"),
            };

            return result;
        }


        protected DocumentationLibrary CreateTestLibrary()
        {
            var configuration = ConfigurationUtility.GetConfiguration();

            DocumentationLibrary result = new DocumentationLibrary()
            {
                DatabaseConnectionString = configuration.GetConnectionString("TestDb_ConnectionString")!,
                DocumentsDirectory = new DirectoryInfo($@"{CodeUtility.GetProjectRoot()}\Sereno.Documentation.Test\DocumentsLibrary"),
                HtmlExportDirectory = new DirectoryInfo($@"{CodeUtility.GetDataDirectory()}\Sereno.Office\"),
            };

            return result;
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
