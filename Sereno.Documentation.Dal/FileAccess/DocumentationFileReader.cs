using DocumentFormat.OpenXml.Packaging;
using Sereno.Office.Word;
using Sereno.Office.Word.SimpleStructure;
using Sereno.Office.Word.Word.SimpleStructure.Converter;
using Sereno.Utilities;
using System.Data;

namespace Sereno.Documentation.FileAccess
{
    public class DocumentationFileReader
    {
        public static DocumentationFile? Read(DocumentationReaderOptions options)
        {
            DocumentationFile? result = null;

            using WordprocessingDocument document = WordUtility.OpenWordDocument(options.FilePath);

            List<DocumentGroup> allGroups = [.. DocumentGroupUtility.GetDocumentGroups(document)];

            result = new()
            {
                File = new FileInfo(options.FilePath),
            };

            // Titel
            ParagraphGroup? title = GetTitle(allGroups);

            if (title != null)
            {
                result.Title = title.PlainText;
            }

            // Dokumentations Daten
            TableGroup? tableGroup = GetDocumentDataTable(allGroups);
            if (tableGroup != null)
            {
                result.HasDocumentationData = true;

                TableInfoOptions tableInfoOptions = new TableInfoOptions()
                {
                    DetermineHeaderRow = false,
                    HasHeaderRow = false,
                };
                TableInfo tableInfo = TableGroupUtility.GetTableInfo(tableGroup, tableInfoOptions);

                MapDocumentationData(tableInfo.Data, result);

                result.Contents = GetContentGroups(allGroups, tableGroup, title);
            }

            // Plain Text
            result.PlainText = String.Join(Environment.NewLine, result.Contents.Select(x => x.PlainText));


            return result;
        }


        /// <summary>
        /// Im Dokument steht oben ein Header und danach der Content
        /// Hier liefern wir nur die Gruppen, die zum Content gehören
        /// </summary>
        private static List<DocumentGroup> GetContentGroups(List<DocumentGroup> allGroups, TableGroup? documentDataTable, ParagraphGroup? title)
        {
            List<DocumentGroup> result = [];

            bool contentAfterTableReached = false;

            if (documentDataTable == null)
                contentAfterTableReached = true;

            bool startGroupAdding = false;

            foreach (DocumentGroup group in allGroups)
            {
                // zuerst warten wir, bis wir die Tabelle mit den Dokumentationsaten passiert haben
                if (group == documentDataTable)
                    contentAfterTableReached = true;

                if (contentAfterTableReached)
                {
                    // dann warten wir bis zum ersten Paragraphen, der etwas beinhaltet (keine Leeerzeilen)
                    if (String.IsNullOrWhiteSpace(group.PlainText))
                    {
                        startGroupAdding = true;
                    }
                }

                if (startGroupAdding)
                {
                    result.Add(group);
                }
            }

            // Falls wir einen Titel haben, kommt der wieder ganz zu Beginn rein
            if (title != null)
            {
                result.Insert(0, title);
            }

            return result;
        }



        private static void MapDocumentationData(DataTable table, DocumentationFile file)
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
