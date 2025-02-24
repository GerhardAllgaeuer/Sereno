using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Sereno.Office.Word.SimpleStructure;
using Sereno.Utilities;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Xml.Linq;

namespace Sereno.Office.Word
{
    public class WordUtility
    {


        /// <summary>
        /// Word nimmt sich einen eklusiven Lock auf das File. Somit muss das File immer geschlossen sein. 
        /// Hier umgehen wir das.
        /// </summary>
        public static WordprocessingDocument OpenWordDocument(string filePath)
        {
            WordprocessingDocument result;

            using FileStream fileStream = new(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            result = WordprocessingDocument.Open(fileStream, false);

            return result;
        }


        /// <summary>
        /// Dokument öffnen
        /// </summary>
        public static void OpenDocument(string filePath)
        {
            try
            {
                Process.Start(new ProcessStartInfo(filePath) { UseShellExecute = true });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ein Fehler ist aufgetreten: {ex.Message}");
            }
        }




        /// <summary>
        /// Text in einem Word-Dokument suchen
        /// </summary>
        public static IEnumerable<Text> FindTextInWordDocument(WordprocessingDocument document, string searchText)
        {
            List<Text> foundTexts = [];

            var texts = document?.MainDocumentPart?.Document?.Body?.Descendants<Text>();
            if (texts != null)
            {
                foreach (var textElement in texts)
                {
                    if (textElement.Text.Contains(searchText))
                    {
                        foundTexts.Add(textElement);
                    }
                }
            }

            return foundTexts;
        }



        /// <summary>
        /// Text an ein OpenXmlElement anhängen
        /// </summary>
        public static Paragraph AddParagraph(OpenXmlElement element, string text, TextFormatOptions? options = null)
        {
            Paragraph? result;

            text = "\u00A0" + text;

            if (element is Body || element is TableCell)
            {
                result = new Paragraph();
                FormatParagraph(result, options);
                var run = new Run(new Text(text));
                result.Append(run);
                element.Append(result);
            }
            if (element is Paragraph existing)
            {
                result = new Paragraph();
                var run = new Run(new Text(text));
                FormatParagraph(result, options);
                result.Append(run);
                existing.InsertAfterSelf(result);
            }
            else
            {
                result = new Paragraph(new Run(new Text(text)));
                FormatParagraph(result, options);
                element.InsertAfterSelf(result);
            }

            return result;
        }



        /// <summary>
        /// Text an ein OpenXmlElement anhängen
        /// </summary>
        public static void AddText(OpenXmlElement element, string text, TextFormatOptions? options = null)
        {
            // Erstellen eines neuen Paragraphen, wenn das Element ein Container für Paragraphen ist
            if (element is Body || element is TableCell)
            {
                var para = new Paragraph();
                var run = new Run(new Text(text));
                para.Append(run);
                FormatParagraph(para, options);
                element.Append(para);
            }
            else if (element is Paragraph para)
            {
                // Direktes Hinzufügen des Texts zu einem bestehenden Paragraphen
                var run = new Run(new Text(text));
                para.Append(run);
                FormatParagraph(para, options);
            }
            else
            {
                // Für alle anderen Elementtypen, fügen Sie den Text als Run hinzu, falls möglich
                var run = new Run(new Text(text));
                element.Append(run);
            }
        }

        private static void FormatParagraph(Paragraph paragraph, TextFormatOptions? options)
        {
            if (options?.Style != null)
            {
                ParagraphProperties paragraphProperties = new ParagraphProperties(
                                       new ParagraphStyleId() { Val = options.Style });
                paragraph.Append(paragraphProperties);
            }
        }



        /// <summary>
        /// Columns aus DataTable erstellen (mit allen Spalten)
        /// </summary>
        private static List<ColumnOption> CreateColumnOptionsFromDataTable(DataTable dataTable)
        {
            List<ColumnOption> columnOptions = [];


            foreach (DataColumn column in dataTable.Columns)
            {
                columnOptions.Add(new ColumnOption()
                {
                    SourceName = column.ColumnName,
                });
            }

            return columnOptions;
        }


        /// <summary>
        /// DataSet Werte in eine Word-Tabelle schreiben
        /// </summary>
        public static void FillTable(WordprocessingDocument document, DataTable dataTable, string bookmarkName, TableOption? options)
        {
            if (document != null &&
                document.MainDocumentPart != null &&
                document.MainDocumentPart.Document != null)
            {

                // Standardoptionen, falls keine übergeben wurden
                if (options == null)
                    options = new TableOption();

                // Spaltenoptionen erstellen, falls keine übergeben wurden
                if (options.ColumnOptions == null ||
                    options.ColumnOptions.Count == 0)
                    options.ColumnOptions = CreateColumnOptionsFromDataTable(dataTable);


                var doc = document.MainDocumentPart.Document;

                // Finde die Textmarke im Dokument
                BookmarkStart? bookmarkStart = doc?.Body?.Descendants<BookmarkStart>().FirstOrDefault(b => b.Name == bookmarkName);
                if (bookmarkStart == null)
                {
                    throw new ArgumentException("Textmarke nicht gefunden.", nameof(bookmarkName));
                }

                // Finde die Tabelle in der Textmarke
                Table? table = bookmarkStart?.Ancestors<Table>().FirstOrDefault();
                if (table == null)
                {
                    throw new ArgumentException("Keine Tabelle an der Textmarke gefunden.", nameof(bookmarkName));
                }

                // Wenn die Tabelle einen Header hat, überspringe die erste Zeile beim Einfügen/Bearbeiten
                int effectiveStartRow = options.HasHeader ? options.StartRow : options.StartRow;

                var rows = table.Elements<TableRow>().ToList();
                if (effectiveStartRow > rows.Count)
                {
                    throw new ArgumentOutOfRangeException(nameof(options.StartRow), "Startreihe liegt außerhalb der vorhandenen Tabelle.");
                }

                // Spaltenoptionen in ein Dictionary umwandeln
                // Aus Performancegründen machen wir das vor der Schleife
                Dictionary<string, ColumnOption> columnDictionary = DictionaryUtility.CreateDictionaryFromList<ColumnOption, string>(options.ColumnOptions, nameof(ColumnOption.SourceName));


                // Wir merken uns für die Formatierung der Texte eine Referenzzeile
                TableRow referenceRow = options.HasHeader && table.Elements<TableRow>().Count() > 1
                    ? table.Elements<TableRow>().ElementAt(1)
                    : table.Elements<TableRow>().First();

                for (int i = 0; i < dataTable.Rows.Count; i++)
                {

                    TableRow row;
                    if (effectiveStartRow + i < rows.Count)
                    {
                        row = rows[effectiveStartRow];
                        UpdateTableRowFromDataRow(row, dataTable.Rows[i], options, columnDictionary, referenceRow);
                    }
                    else
                    {
                        row = new TableRow();
                        table.Append(row);
                        UpdateTableRowFromDataRow(row, dataTable.Rows[i], options, columnDictionary, referenceRow);
                    }
                }

            }
        }


        private static void UpdateTableRowFromDataRow(TableRow tableRow, DataRow dataRow, TableOption options, Dictionary<string, ColumnOption> dictionary, TableRow referenceRow)
        {
            var referenceCellProperties = referenceRow.Elements<TableCell>().Select(tc => tc.Descendants<RunProperties>().FirstOrDefault()).ToList();

            int cellIndex = 0;
            foreach (DataColumn column in dataRow.Table.Columns)
            {
                string columnName = column.ColumnName;
                if (dictionary.ContainsKey(columnName))
                {
                    object value = dataRow[columnName];
                    ColumnOption columnOption = dictionary[columnName];
                    string formattedValue = FormatValue(value, columnOption);

                    TableCell cell;
                    if (tableRow.Elements<TableCell>().Count() > cellIndex)
                    {
                        cell = tableRow.Elements<TableCell>().ElementAt(cellIndex);
                    }
                    else
                    {
                        cell = CreateTableCellWithFormatting(referenceCellProperties.ElementAtOrDefault(cellIndex));
                        tableRow.Append(cell);
                    }

                    UpdateCellContent(cell, formattedValue, referenceCellProperties.ElementAtOrDefault(cellIndex));
                    cellIndex++;
                }
            }
        }

        private static void UpdateCellContent(TableCell cell, string content, RunProperties? referenceRunProperties)
        {
            Paragraph p = cell.Elements<Paragraph>().FirstOrDefault() ?? cell.AppendChild(new Paragraph());
            Run r = p.Elements<Run>().FirstOrDefault() ?? p.AppendChild(new Run());

            if (referenceRunProperties != null)
            {
                r.RunProperties = (RunProperties)referenceRunProperties.CloneNode(true);
            }

            Text? t = r.Elements<Text>().FirstOrDefault();
            if (t == null)
            {
                t = new Text(content);
                r.Append(t);
            }
            else
            {
                t.Text = content;
            }
        }


        private static TableCell CreateTableCellWithFormatting(RunProperties? referenceRunProperties)
        {
            var cell = new TableCell(new Paragraph(new Run()));
            if (referenceRunProperties != null)
            {
                var run = cell.Descendants<Run>().First();
                run.RunProperties = (RunProperties)referenceRunProperties.CloneNode(true);
            }
            return cell;
        }




        /// <summary>
        /// Einzelnen Wert formatieren
        /// </summary>
        private static string FormatValue(object? value, ColumnOption? columnOption)
        {
            string result = "";
            if (value != null)
            {
                result = value.ToString() ?? "";
            }
            return result;
        }



        /// <summary>
        /// Bookbarks suchen, Bookmark-Inhalt löschen und zurückgeben
        /// </summary>
        public static BookmarkStart? GetBookmark(WordprocessingDocument? document, string bookmarkName)
        {
            return GetBookmarks(document, bookmarkName).FirstOrDefault();
        }


        /// <summary>
        /// Bookbarks suchen, Bookmark-Inhalt löschen und zurückgeben
        /// </summary>
        public static List<BookmarkStart> GetBookmarks(WordprocessingDocument? document, string bookmarkName)
        {
            var bookmarks = new List<BookmarkStart>();

            if (document == null) return bookmarks;

            MainDocumentPart? mainPart = document.MainDocumentPart;

            if (mainPart?.Document?.Body == null) return bookmarks;

            var bookmarksStart = mainPart.Document.Body.Descendants<BookmarkStart>()
                .Where(b => b.Name == bookmarkName).ToList();

            foreach (var bookmarkStart in bookmarksStart)
            {
                BookmarkEnd? bookmarkEnd = mainPart.Document.Body.Descendants<BookmarkEnd>()
                    .FirstOrDefault(b => b.Id == bookmarkStart.Id);

                if (bookmarkEnd == null) continue;

                // Identifizieren und Entfernen des existierenden Inhalts zwischen BookmarkStart und BookmarkEnd
                OpenXmlElement? nextElem = bookmarkStart.NextSibling();
                while (nextElem != null && nextElem != bookmarkEnd)
                {
                    OpenXmlElement? currentElem = nextElem;
                    nextElem = nextElem.NextSibling();
                    currentElem.Remove();
                }

                bookmarks.Add(bookmarkStart);
            }

            return bookmarks;
        }
    }
}
