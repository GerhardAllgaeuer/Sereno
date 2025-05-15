using System;

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
        public string ColumnName { get; set; } = string.Empty;


        /// <summary>
        /// Property vom Objekt, dem gemappt wird
        /// </summary>
        public string SourceProperty { get; set; } = string.Empty;


        /// <summary>
        /// Booleans in einen String mit x umwandeln
        /// </summary>
        public bool ConvertBoolToX { get; set; }

        public MappingColumn()
        {
        }

        public MappingColumn(string columnName, string sourceProperty)
        {
            ColumnName = columnName ?? throw new ArgumentNullException(nameof(columnName));
            SourceProperty = sourceProperty ?? throw new ArgumentNullException(nameof(sourceProperty));
        }
    }
}
