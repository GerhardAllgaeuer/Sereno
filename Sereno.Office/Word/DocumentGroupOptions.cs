using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sereno.Office.Word
{
    public class DocumentGroupOptions
    {
        /// <summary>
        /// Filter, der angibt, ob ein Absatz in eine Gruppe aufgenommen wird
        /// </summary>
        public string? ParagraphStyleFilter { get; set; }

        /// <summary>
        /// Text extrahieren
        /// </summary>
        public bool ExtractInnerText { get; set; } = false;
    }
}
