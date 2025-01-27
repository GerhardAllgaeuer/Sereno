using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Sereno.Database;
using Sereno.Documentation.DataAccess;
using Sereno.Documentation.DataAccess.Entities;

namespace Sereno.Documentation
{
    [TestClass]
    public sealed class DatabaseUpdateTest
    {
        private string connectionString = "";


        [TestInitialize]
        public void Setup()
        {
            var configuration = ConfigurationHelper.GetConfiguration();
            connectionString = configuration.GetConnectionString("Documentation_ConnectionString")!;
        }


        [TestMethod]
        public void Update_Data_Auto()
        {
            using var context = AppDbContext.Create(connectionString, "inserter@test.com");

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



            using var updatecontext = AppDbContext.Create(connectionString, "updater@test.com");

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
