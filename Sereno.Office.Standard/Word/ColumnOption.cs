using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sereno.Office.Word
{
    public class ColumnOption
    {
        /// <summary>
        /// Bezeichnung / Titel der Spalte aus dem Quell-Objekt
        /// </summary>
        public string SourceName { get; set; }

        /// <summary>  
        /// Formatierungsstring für die Spalte
        /// </summary>
        public string FormatString { get; set; }
    }
}
