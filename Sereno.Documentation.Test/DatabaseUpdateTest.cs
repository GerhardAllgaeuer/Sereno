using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sereno.Documentation.DataAccess.Entities;
using Sereno.Documentation.Test;

namespace Sereno.Documentation
{
    [TestClass]
    public sealed class DatabaseUpdateTest : DocumentationTestBase
    {

        [TestMethod]
        public void Update_Data_Auto()
        {
            using var context = DbContextFactory.CreateTestDb(appContext);

            string id = Guid.NewGuid().ToString();

            // Neue Document-Entität erstellen
            var document = new Document
            {
                Id = id,
                LibraryPath = @"\Abc\Def",
                DocumentKey = "Document0001",
                Title = "Erstes Dokument",
                Content = "Dies ist der Inhalt des Dokuments.",
            };

            // Hinzufügen und speichern
            context.Documents.Add(document);
            context.SaveChanges();

            Console.WriteLine(connectionString);


            using var updatecontext = DbContextFactory.CreateTestDb(appContext);

            // Ändere den Datensatz
            var existingDocument = updatecontext.Documents.FirstOrDefault(d => d.Id == id);

            if (existingDocument != null)
            {
                existingDocument.Title = "Geändertes Dokument";
                existingDocument.Content = "Das ist der geänderte Inhalt.";

                // Änderungen speichern
                updatecontext.SaveChanges();
            }
        }

    }
}
