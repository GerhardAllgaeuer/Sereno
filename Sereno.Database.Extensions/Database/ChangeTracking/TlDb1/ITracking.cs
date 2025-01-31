using System.ComponentModel.DataAnnotations.Schema;

namespace Sereno.Database.ChangeTracking.TlDb1
{
    public interface ITracking
    {
        public DateTime? Create { get; set; }

        [Column(TypeName = "nvarchar(500)")]
        public string? CreateUser { get; set; }

        public DateTime? Modify { get; set; }

        [Column(TypeName = "nvarchar(500)")]
        public string? ModifyUser { get; set; }
    }
}
