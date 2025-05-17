using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Sereno.System.DataAccess.Entities;
using Sereno.System.Test;

namespace Sereno.System
{
    [TestClass]
    public sealed class DatabaseEntitiesTests : SystemTestBase
    {

        [TestMethod]
        public void Create_Data_Auto()
        {
            using var context = DbContextFactory.CreateTestDb(appContext);


            // User, Roles

            var user = new User
            {
                Id = "TestUser",
                Description = "TestUser",
                Email = "test@sereno.com",
                StateId = State.Active.Id
            };
            context.Users.Add(user);

            var role = new Role
            {
                Id = "Administrator",
            };
            context.Roles.Add(role);

            var userRole = new UserRole
            {
                Id = Guid.NewGuid().ToString(),
                UserId = user.Id,
                RoleId = role.Id,
            };
            context.UserRoles.Add(userRole);


            // Devices

            var device = new Device
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Computer01",
            };
            context.Devices.Add(device);


            // Products, Configs

            var product = new Product
            {
                Id = "ClientUserApp",
                Description = "User Context Application ",
            };
            context.Products.Add(product);


            var jsonData = new
            {
                name = "Max",
                age = 30,
                skills = new[] { "SQL", "C#" }
            };

            var config = new Config
            {
                Id = Guid.NewGuid().ToString(),
                DeviceId = device.Id,
                UserId = user.Id,
                ProductId = product.Id,
                Data = JsonConvert.SerializeObject(jsonData),
            };
            context.ProductConfigs.Add(config);


            

            context.SaveChanges();
        }
    }
}
