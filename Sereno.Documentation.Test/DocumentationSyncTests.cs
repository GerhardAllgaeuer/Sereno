using Sereno.Documentation.FileAccess;
using Sereno.Utilities;
using Microsoft.Extensions.Configuration;
using Sereno.Documentation.Synchronization;

namespace Sereno.Documentation
{
    [TestClass]
    public sealed class DocumentationSyncTests
    {
        [TestMethod]
        public void Sync_Library()
        {
            var configuration = ConfigurationUtility.GetConfiguration();

            SyncOptions options = new()
            {
                DatabaseConnectionString = configuration.GetConnectionString("TestDb_ConnectionString")!,
                DocumentsDirectory = new DirectoryInfo($@"{CodeUtility.GetProjectRoot()}\Sereno.Documentation.Test\DocumentsLibrary"),
                HtmlExportDirectory = new DirectoryInfo($@"{CodeUtility.GetDataDirectory()}\Sereno.Office\"),
            };

            DocumentationLibraryUtility.SyncLibrary(options);
        }
    }
}
