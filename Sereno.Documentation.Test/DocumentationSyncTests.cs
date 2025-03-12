using Sereno.Utilities;
using Microsoft.Extensions.Configuration;
using Sereno.Documentation.Synchronization;
using Sereno.Documentation.Test;

namespace Sereno.Documentation
{
    [TestClass]
    public sealed class DocumentationSyncTests : DocumentationTestBase
    {
        [TestMethod]
        public void Sync_Library()
        {
            SyncOptions options = new()
            {
            };

            DocumentationLibrary library = this.CreateTestLibrary();

            library.CleanupHtmlExportDirectory();

            library.SyncLibrary(options);
        }




        [TestMethod]
        [TestProperty("Dev", "")]
        public void Sync_Production_Structure()
        {
            SyncOptions options = new()
            {
            };

            DocumentationLibrary library = this.CreateDocumentationLibrary();
            library.SyncLibrary(options);
        }
    }
}
