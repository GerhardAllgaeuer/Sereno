using FluentAssertions;
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

            library.CleanupTargetFilesDirectory();
            library.DeleteAllDocumentsInDatabase();
            library.SyncLibrary(options);

            File.Exists($@"{library.TargetFilesDirectory!.FullName}\Topic1\Documentation_0001\Image0001.png").Should().BeTrue("Image0001.png nicht vorhanden");

        }




        [TestMethod]
        [TestProperty("Dev", "")]
        public void Sync_Production_Library()
        {
            SyncOptions options = new()
            {
            };

            DocumentationLibrary library = this.CreateDocumentationLibrary();
            library.DeleteAllDocumentsInDatabase();
            library.CleanupTargetFilesDirectory();
            library.SyncLibrary(options);
        }
    }
}
