using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public string? StyleId { get; internal set; }

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
        public string? InnerText { get; set; }


    }
}
