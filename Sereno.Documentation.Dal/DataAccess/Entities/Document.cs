using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO.Pipelines;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sereno.Documentation.DataAccess.Entities
{
    public class Document
    {
        [Column(TypeName = "nvarchar(50)")]
        public required string Id { get; set; }

        [Column(TypeName = "nvarchar(500)")]
        public required string Title { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string? Content { get; set; }

        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }

    }
}
