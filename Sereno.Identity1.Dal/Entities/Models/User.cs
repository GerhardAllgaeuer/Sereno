using Microsoft.AspNetCore.Identity;

namespace Sereno.Identity.Entities.Models
{
    public class User : IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
    }
}
