using Sereno.Office.Word.SimpleStructure;
using System.Collections.Generic;
using System.IO;

namespace Sereno.Office.Word.Word.SimpleStructure.Export
{
    public class ExportOptions
    {
        public DirectoryInfo ExportDirectory { get; set; }

        public List<DocumentGroup> Groups { get; set; }
    }
}
