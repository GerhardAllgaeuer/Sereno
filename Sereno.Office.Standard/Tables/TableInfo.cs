using System.Collections.Generic;
using System.Data;

namespace Sereno.Office.Tables
{

    /// <summary>
    /// Definition einer Tabelle für Excel, Word, usw.
    /// </summary>
    public class TableInfo
    {

        /// <summary>
        /// Name der Tabelle
        /// </summary>
        public string TableName { get; set; } = "Table";


        /// <summary>
        /// Liste mit den Spalten der Tabelle
        /// </summary>
        public List<TableColumn> Columns { get; set; } = new List<TableColumn>();

    }
}
