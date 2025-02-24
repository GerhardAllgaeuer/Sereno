using DocumentFormat.OpenXml.Packaging;
using Sereno.Office.Word;
using Sereno.Office.Word.SimpleStructure;
using DocumentFormat.OpenXml.Wordprocessing;
using System.Data;

namespace Sereno.Documentation.FileAccess
{
    public class DocumentationFileReader
    {

        private static readonly Dictionary<string, Action<DocumentationFile, string>> PropertySetters = new()
        {
            { "Verantwortlich", (obj, value) => obj.Verantwortlich = value },
            { "Verantwortlichkeit", (obj, value) => obj.Verantwortlich = value },
            { "Information", (obj, value) => obj.Information = value },
            { "Nächste Prüfung", (obj, value) => obj.NächstePrüfung = DateTime.TryParse(value, out var date) ? date : default },
            { "Typ", (obj, value) => obj.Typ = value }
        };


        public static DocumentationFile Read(string filePath)
        {
            DocumentationFile result = new DocumentationFile()
            {
                File = new FileInfo(filePath),
            };

            using WordprocessingDocument document = WordUtility.OpenWordDocument(filePath);

            List<DocumentGroup> groups = [.. DocumentGroupUtility.GetDocumentGroups(document)];

            ParagraphGroup? title = GetTitle(groups);
            TableGroup? documentDataTable = GetDocumentDataTable(groups);
            if (documentDataTable != null)
            {
                TableInfo tableInfo = TableGroupUtility.GetTableInfo(documentDataTable);
                MapTableToObject(tableInfo.Data, result);
            }

            return result;
        }

        public static void MapTableToObject(DataTable table, DocumentationFile file)
        {
            foreach (DataRow row in table.Rows)
            {
                string key = row[0]?.ToString()?.Trim() ?? "";
                string value = row[1]?.ToString()?.Trim() ?? "";

                if (PropertySetters.TryGetValue(key, out var setter))
                {
                    setter(file, value);
                }
            }
        }

        private static TableGroup? GetDocumentDataTable(List<DocumentGroup> groups)
        {
            for (int i = 0; i < 5; i++)
            {
                if (groups[i] is TableGroup group)
                {
                    return group;
                }
            }

            return null;
        }

        private static ParagraphGroup? GetTitle(List<DocumentGroup> groups)
        {
            for (int i = 0; i < 3; i++)
            {
                if (groups[i] is ParagraphGroup pargraphGroup)
                {
                    if (pargraphGroup.StyleNameEn != null &&
                        pargraphGroup.StyleNameEn == "Title")
                    {
                        return pargraphGroup;
                    }
                }
            }

            return null;
        }

    }
}
