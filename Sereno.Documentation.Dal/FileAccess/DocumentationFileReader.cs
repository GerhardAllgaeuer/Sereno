using DocumentFormat.OpenXml.Packaging;
using Sereno.Office.Word;
using Sereno.Office.Word.SimpleStructure;
using System.Data;

namespace Sereno.Documentation.FileAccess
{
    public class DocumentationFileReader
    {



        public static DocumentationFile? Read(DocumentationReaderOptions options)
        {
            DocumentationFile? result = null;

            using WordprocessingDocument document = WordUtility.OpenWordDocument(options.FilePath);

            List<DocumentGroup> groups = [.. DocumentGroupUtility.GetDocumentGroups(document)];

            result = new()
            {
                File = new FileInfo(options.FilePath),
            };

            // Titel
            ParagraphGroup? title = GetTitle(groups);

            if (title != null)
            {
                result.Title = title.InnerText;
            }

            // Dokumentations Daten
            TableGroup? documentDataTable = GetDocumentDataTable(groups);
            if (documentDataTable != null)
            {
                result.HasDocumentationData = true;

                TableInfoOptions tableInfoOptions = new TableInfoOptions()
                {
                    DetermineHeaderRow = false,
                    HasHeaderRow = false,
                };
                TableInfo tableInfo = TableGroupUtility.GetTableInfo(documentDataTable, tableInfoOptions);

                MapDocumentationData(tableInfo.Data, result);
            }

            return result;
        }

        public static void MapDocumentationData(DataTable table, DocumentationFile file)
        {
            foreach (DataRow row in table.Rows)
            {
                string key = row[0]?.ToString()?.Trim() ?? "";
                string value = row[1]?.ToString()?.Trim() ?? "";

                try
                {
                    switch (key)
                    {
                        case "Verantwortlich":
                        case "Verantwortlichkeit":
                            if (string.IsNullOrEmpty(file.Author)) file.Author = value;
                            break;
                        case "Information":
                            if (string.IsNullOrEmpty(file.InfoReceivers)) file.InfoReceivers = value;
                            break;
                        case "Nächste Prüfung":
                            if (!file.NextCheck.HasValue && DateTime.TryParse(value, out var date)) file.NextCheck = date;
                            break;
                        case "Typ":
                            if (string.IsNullOrEmpty(file.Type)) file.Type = value;
                            break;
                    }
                }
                catch
                {
                    throw new Exception($"Error mapping {key} in {file.File.Name}");
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
