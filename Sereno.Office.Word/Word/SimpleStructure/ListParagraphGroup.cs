
using DocumentFormat.OpenXml.Wordprocessing;
using Sereno.Office.Word.Word.SimpleStructure;
using System.Collections.Generic;

namespace Sereno.Office.Word.SimpleStructure
{

    public class ListParagraphGroup : DocumentGroup
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
        /// Hierarchsiche Klasse mit Listen-Elementen
        /// </summary>
        public List<ListParagraph> ListParagraphs { get; set; } = new List<ListParagraph>();


        /// <summary>
        /// Beschreibung der Gruppe
        /// </summary>
        public override string ToString()
        {
            string result = "";
            if (!string.IsNullOrWhiteSpace(InnerText))
            {
                if (InnerText.Length > 20)
                    result = InnerText.Substring(0, 17) + "...";
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
