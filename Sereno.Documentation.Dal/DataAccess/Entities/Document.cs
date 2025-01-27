using System.ComponentModel.DataAnnotations.Schema;

namespace Sereno.Documentation.DataAccess.Entities
{
    [Table("docDocument")]
    public class Document
    {
        [Column(TypeName = "nvarchar(50)")]
        public required string Id { get; set; }


        [Column(TypeName = "nvarchar(500)")]
        public string? Title { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string? Content { get; set; }



        public DateTime? Create { get; set; }

        [Column(TypeName = "nvarchar(500)")]
        public string? CreateUser { get; set; }

        public DateTime? Modify { get; set; }

        [Column(TypeName = "nvarchar(500)")]
        public string? ModifyUser { get; set; }

    }
}
