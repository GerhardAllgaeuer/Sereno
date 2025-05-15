using System.ComponentModel.DataAnnotations.Schema;
using Sereno.Database.Logging.TlDb1;

namespace Sereno.System.DataAccess.Entities
{
    [Table("sysConfig")]
    public class Config : ILogging
    {
        [Column(TypeName = "nvarchar(50)")]
        public required string Id { get; set; }


        [Column(TypeName = "nvarchar(500)")]
        public required string DeviceId { get; set; }

        [ForeignKey("DeviceId")]
        public Device Device { get; set; } = null!;

        [Column(TypeName = "nvarchar(50)")]
        public required string UserId { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; } = null!;

        [Column(TypeName = "nvarchar(200)")]
        public required string ProductId { get; set; }

        [ForeignKey("ProductId")]
        public Product Product { get; set; } = null!;


        [Column(TypeName = "nvarchar(max)")]
        public required string Data { get; set; }

        public DateTime? Create { get; set; }

        [Column(TypeName = "nvarchar(500)")]
        public string? CreateUser { get; set; }

        public DateTime? Modify { get; set; }

        [Column(TypeName = "nvarchar(500)")]
        public string? ModifyUser { get; set; }
    }
}
