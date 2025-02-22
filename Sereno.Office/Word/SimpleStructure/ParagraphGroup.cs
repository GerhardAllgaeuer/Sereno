
using DocumentFormat.OpenXml.Wordprocessing;

namespace Sereno.Office.Word.SimpleStructure
{

    public class ParagraphGroup : DocumentGroup
    {
        /// <summary>
        /// Stil-ID
        /// </summary>
        public string StyleId { get; internal set; } = "";

        /// <summary>
        /// Stil-Namen auf Englisch
        /// </summary>
        public string StyleNameEn { get; internal set; } = "";

        /// <summary>
        /// Stil-Namen
        /// </summary>
        public string StyleName { get; internal set; } = "";


        /// <summary>
        /// Liste mit Paragraphen
        /// </summary>
        public List<Paragraph> Paragraphs { get; set; } = [];



        /// <summary>
        /// Beschreibung der Gruppe
        /// </summary>
        public override string ToString()
        {
            string result = "";
            if (!string.IsNullOrWhiteSpace(InnerText))
            {
                if (InnerText.Length > 20)
                    result = InnerText[..17] + "...";
                else
                    result = InnerText;
            }

            if (!string.IsNullOrWhiteSpace(StyleName))
            {
                if (!string.IsNullOrWhiteSpace(result))
                {
                    result += " - ";
                }

                result += StyleName;
            }
            return result;
        }
    }
}
