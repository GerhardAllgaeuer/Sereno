using System.Collections.Generic;

namespace Sereno.Utilities.TableConverter
{

    /// <summary>
    /// Definition einer Tabelle für Excel, Word, usw.
    /// </summary>
    public class MappingInfo
    {

        /// <summary>
        /// Name der Tabelle
        /// </summary>
        public string TableName { get; set; } = "Table";


        /// <summary>
        /// Liste mit den Spalten der Tabelle
        /// </summary>
        public List<MappingColumn> Columns { get; set; } = new List<MappingColumn>();

    }
}
