using System.ComponentModel.DataAnnotations.Schema;
using Sereno.Database.Logging.TlDb1;

namespace Sereno.System.DataAccess.Entities
{
    [Table("syrUsrGrp")]
    public class UserRole : ILogging
    {
        [Column(TypeName = "nvarchar(50)")]
        public required string Id { get; set; }

        [Column(TypeName = "nvarchar(200)")]
        public required string UserId { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; } = null!;

        [Column(TypeName = "nvarchar(200)")]
        public required string RoleId { get; set; }

        [ForeignKey("RoleId")]
        public Role Role { get; set; } = null!;


        public DateTime? Create { get; set; }

        [Column(TypeName = "nvarchar(500)")]
        public string? CreateUser { get; set; }

        public DateTime? Modify { get; set; }

        [Column(TypeName = "nvarchar(500)")]
        public string? ModifyUser { get; set; }
    }
}
