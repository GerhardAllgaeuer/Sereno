using System.ComponentModel.DataAnnotations.Schema;
using Sereno.Database.ChangeTracking.TlDb1;

namespace Sereno.TlDb1.DataAccess.Entities
{
    [Table("tstSimple")]
    public class SimpleTable : ITracking
    {

        [Column(TypeName = "nvarchar(50)")]
        public required string Id { get; set; }




        [Column(TypeName = "nvarchar(500)")]
        public string? Title { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string? Description { get; set; }




        public DateTime? Create { get; set; }

        [Column(TypeName = "nvarchar(500)")]
        public string? CreateUser { get; set; }

        public DateTime? Modify { get; set; }

        [Column(TypeName = "nvarchar(500)")]
        public string? ModifyUser { get; set; }

    }
}
