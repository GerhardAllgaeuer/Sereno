using System.ComponentModel.DataAnnotations.Schema;
using Sereno.Database.Logging.TlDb1;

namespace Sereno.Documentation.DataAccess.Entities
{
    [Table("docDocument")]
    public class Document : ILogging
    {

        [Column(TypeName = "nvarchar(50)")]
        public required string Id { get; set; }


        [Column(TypeName = "nvarchar(1000)")]
        public required string LibraryPath { get; set; }


        [Column(TypeName = "nvarchar(500)")]
        public required string DocumentKey { get; set; }



        [Column(TypeName = "nvarchar(1000)")]
        public string? Title { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string? Content { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string? HtmlContent { get; set; }



        [Column(TypeName = "nvarchar(1000)")]
        public string? Author { get; set; }


        public DateTime? NextCheck { get; set; }


        public DateTime? Create { get; set; }

        [Column(TypeName = "nvarchar(500)")]
        public string? CreateUser { get; set; }

        public DateTime? Modify { get; set; }

        [Column(TypeName = "nvarchar(500)")]
        public string? ModifyUser { get; set; }

    }
}
