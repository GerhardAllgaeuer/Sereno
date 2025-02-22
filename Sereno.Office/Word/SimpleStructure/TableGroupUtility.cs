using DocumentFormat.OpenXml.Wordprocessing;

namespace Sereno.Office.Word.SimpleStructure
{
    public class TableGroupUtility
    {

        /// <summary>
        /// Tabellen Gruppen Objekt erstellen
        /// </summary>
        public static TableGroup ProcessTable(Table table)
        {
            TableGroup result = new()
            {
                Table = table,
            };

            return result;
        }

    }
}
