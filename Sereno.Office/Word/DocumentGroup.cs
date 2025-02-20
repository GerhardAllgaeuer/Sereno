using DocumentFormat.OpenXml.Wordprocessing;

namespace Sereno.Office.Word
{

    /// <summary>
    /// Gruppe von Absätzen in einem Word-Dokument
    /// </summary>
    public class DocumentGroup
    {
        /// <summary>
        /// Liste von Absätzen
        /// </summary>
        public List<Paragraph> Paragraphs { get; set; } = [];

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
        /// Anschließende Gruppe
        /// </summary>
        public DocumentGroup? NextGroup { get; set; }

        /// <summary>
        /// Gruppe davor
        /// </summary>
        public DocumentGroup? PreviousGroup { get; set; }


        /// <summary>
        /// Text aus der Gruppe
        /// </summary>
        public string InnerText { get; set; } = "";


        public override string ToString()
        {
            string result = "";
            if (!String.IsNullOrWhiteSpace(this.InnerText))
            {
                if (this.InnerText.Length > 20)
                    result = this.InnerText[..17] + "...";
                else
                    result = this.InnerText;
            }

            if (!String.IsNullOrWhiteSpace(this.StyleName))
            {
                if (!String.IsNullOrWhiteSpace(result))
                {
                    result += " - ";
                }

                result += this.StyleName;
            }
            return result;
        }


    }
}
