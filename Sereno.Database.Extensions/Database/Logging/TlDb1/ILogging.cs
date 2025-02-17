using System.ComponentModel.DataAnnotations.Schema;

namespace Sereno.Database.Logging.TlDb1
{
    public interface ILogging
    {
        public DateTime? Create { get; set; }

        [Column(TypeName = "nvarchar(500)")]
        public string? CreateUser { get; set; }

        public DateTime? Modify { get; set; }

        [Column(TypeName = "nvarchar(500)")]
        public string? ModifyUser { get; set; }
    }
}
