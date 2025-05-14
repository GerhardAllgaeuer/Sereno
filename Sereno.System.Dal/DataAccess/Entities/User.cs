using System.ComponentModel.DataAnnotations.Schema;
using Sereno.Database.Logging.TlDb1;

namespace Sereno.Identity.DataAccess.Entities
{
    [Table("syrUsr")]
    public class User : ILogging
    {

        [Column(TypeName = "nvarchar(50)")]
        public required string Id { get; set; }


        [Column(TypeName = "nvarchar(200)")]
        public required string UserName { get; set; }


        [Column(TypeName = "nvarchar(1000)")]
        public string? Hash { get; set; }

        [Column(TypeName = "nvarchar(max)")]

        public DateTime? Create { get; set; }

        [Column(TypeName = "nvarchar(500)")]
        public string? CreateUser { get; set; }

        public DateTime? Modify { get; set; }

        [Column(TypeName = "nvarchar(500)")]
        public string? ModifyUser { get; set; }

    }
}
