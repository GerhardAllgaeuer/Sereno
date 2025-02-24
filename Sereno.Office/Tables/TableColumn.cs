namespace Sereno.Office.Tables
{

    /// <summary>
    /// Definition einer Tabellenspalte für Excel, Word, usw.
    /// </summary>
    public class TableColumn
    {

        /// <summary>
        /// Bezeichnung / Titel der Spalte
        /// </summary>
        public string? ColumnName { get; set; }


        /// <summary>
        /// Property vom Objekt, dem gemappt wird
        /// </summary>
        public string? SourceProperty { get; set; }

    }
}
