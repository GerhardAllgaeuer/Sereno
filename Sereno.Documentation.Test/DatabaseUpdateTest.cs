using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Sereno.Documentation.DataAccess;
using Sereno.Documentation.DataAccess.Entities;
using Sereno.Documentation.Test;
using Sereno.Utilities;

namespace Sereno.Documentation
{
    [TestClass]
    public sealed class DatabaseUpdateTest : DatabaseTestBase
    {

        [TestMethod]
        public void Update_Data_Auto()
        {

            Console.WriteLine(connectionString);

            using var context = TestDbContextFactory.Create(appContext);

            string id = Guid.NewGuid().ToString();

            // Neue Document-Entität erstellen
            var document = new Document
            {
                Id = id,
                Title = "Erstes Dokument",
                Content = "Dies ist der Inhalt des Dokuments.",
            };

            // Hinzufügen und speichern
            context.Documents.Add(document);
            context.SaveChanges();

            Console.WriteLine(connectionString);


            using var updatecontext = TestDbContextFactory.Create(appContext);

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
