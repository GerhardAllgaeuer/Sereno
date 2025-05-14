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

            var user = new User
            {
                Id = Guid.NewGuid().ToString(),
                Name = "TestUser"
            };
            context.Users.Add(user);


            var role = new Role
            {
                Id = "Administrator",
            };
            context.Users.Add(user);


            var userRole = new UserRole
            {
                Id = Guid.NewGuid().ToString(),
                UserId = user.Id,
                RoleId = role.Id,
            };

            context.SaveChanges();
        }
    }
}
