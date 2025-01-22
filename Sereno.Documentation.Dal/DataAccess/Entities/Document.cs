using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sereno.Documentation.DataAccess.Entities
{
    public class Document
    {
        public required string Id { get; set; }
        public required string Title { get; set; }
        public string? Content { get; set; }

    }
}
