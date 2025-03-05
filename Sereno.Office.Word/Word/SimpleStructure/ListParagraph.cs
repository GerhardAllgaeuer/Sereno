using DocumentFormat.OpenXml.Wordprocessing;
using System.Collections.Generic;

namespace Sereno.Office.Word.Word.SimpleStructure
{

    public class ListParagraph
    {
        public string InnerText { get; set; }
        public int IndentLevel { get; set; }
        public List<ListParagraph> Children { get; set; }
        public Paragraph Paragraph { get; set; }
        public int NumberingId { get; set; } 
        public bool IsNumbered { get; set; }

        public override string ToString()
        {
            return this.InnerText;
        }
    }
}
