using DocumentFormat.OpenXml.Wordprocessing;

namespace Sereno.Office.Word.SimpleStructure
{

    public class TableGroup : DocumentGroup
    {
        public Table Table { get; set; } = [];

    }
}
