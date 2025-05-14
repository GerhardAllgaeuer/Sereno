using Sereno.System.DataAccess.Entities;
using Sereno.System.Test;

namespace Sereno.System
{
    [TestClass]
    public sealed class DatabaseUpdateTest : SystemTestBase
    {

        [TestMethod]
        public void Update_Data_Auto()
        {
            using var context = DbContextFactory.CreateTestDb(appContext);

            string id = "TestUser";

            // Neue Document-Entität erstellen
            var user = new User
            {
                Id = id,
            };

            // Hinzufügen und speichern
            context.Users.Add(user);
            context.SaveChanges();
        }
    }
}
