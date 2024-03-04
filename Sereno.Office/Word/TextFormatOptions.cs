using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sereno.Office.Word
{
    public class TextFormatOptions
    {

        /// <summary>
        /// Einrückung
        /// </summary>  
        public int Identation { get; set; }

        /// <summary>
        /// Formatierungsstil des Textes
        /// </summary>
        public string? Style { get; set; }

    }
}
