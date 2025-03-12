
using DocumentFormat.OpenXml.Wordprocessing;
using System.Collections.Generic;

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
        public List<Paragraph> Paragraphs { get; set; } = new List<Paragraph>();



        /// <summary>
        /// Beschreibung der Gruppe
        /// </summary>
        public override string ToString()
        {
            string result = "";
            if (!string.IsNullOrWhiteSpace(PlainText))
            {
                if (PlainText.Length > 20)
                    result = PlainText.Substring(0, 17) + "...";
                else
                    result = PlainText;
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
