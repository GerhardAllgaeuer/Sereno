
using DocumentFormat.OpenXml.Wordprocessing;

namespace Sereno.Office.Word.SimpleStructure
{

    public class ParagraphGroup : DocumentGroup
    {
        public List<Paragraph> Paragraphs { get; set; } = [];
    }
}
