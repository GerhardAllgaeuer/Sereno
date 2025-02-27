namespace Sereno.Utilities.TableConverter
{

    /// <summary>
    /// Definition einer Tabellenspalte für Excel, Word, usw.
    /// </summary>
    public class MappingColumn
    {

        /// <summary>
        /// Bezeichnung / Titel der Spalte
        /// </summary>
        public string ColumnName { get; set; }


        /// <summary>
        /// Property vom Objekt, dem gemappt wird
        /// </summary>
        public string SourceProperty { get; set; }


        /// <summary>
        /// Booleans in einen String mit x umwandeln
        /// </summary>
        public bool ConvertBoolToX { get; set; }

    }
}
