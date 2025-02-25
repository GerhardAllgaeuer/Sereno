using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sereno.Office.Word
{
    public class TableOption
    {
        /// <summary>
        /// Zeile, bei der im Arbeitsblatt gestartet wird. 
        /// Indes 1-basiert.
        /// </summary>
        public int StartRow { get; set; } = 1;


        /// <summary>
        /// Hat die Tabelle einen Header?
        /// </summary>
        public bool HasHeader { get; set; } = true;


        /// <summary>
        /// Optionen für die Spalten
        /// </summary>
        public List<ColumnOption> ColumnOptions { get; set; } = new List<ColumnOption>();
    }
}
