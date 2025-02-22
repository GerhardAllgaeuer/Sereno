using DocumentFormat.OpenXml.Wordprocessing;

namespace Sereno.Office.Word.SimpleStructure
{

    /// <summary>
    /// Gruppe von Absätzen in einem Word-Dokument
    /// </summary>
    public class DocumentGroup
    {
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

    }
}
