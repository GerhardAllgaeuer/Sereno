using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Sereno.Utilities;
using System.Data;
using System.Diagnostics;

namespace Sereno.Office.Word
{
    public class WordUtility
    {

        /// <summary>
        /// Deutsche Styles werden nicht sauber in Deutsch übersetzt, wir helfen mit dem Dictionary nach
        /// </summary>
        static Dictionary<string, string> styleTranslations = new Dictionary<string, string>
        {
            { "berschrift1", "Überschrift 1" },
            { "berschrift2", "Überschrift 2" },
            { "berschrift3", "Überschrift 3" },
            { "berschrift4", "Überschrift 4" },
        };



        /// <summary>
        /// Word nimmt sich einen eklusiven Lock auf das File. Somit muss das File immer geschlossen sein. 
        /// Hier umgehen wir das.
        /// </summary>
        public static WordprocessingDocument? OpenWordDocument(string filePath)
        {
            WordprocessingDocument? result = null;

            using FileStream fileStream = new(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            result = WordprocessingDocument.Open(fileStream, false);

            return result;
        }


        /// <summary>
        /// Text aus den Paragraphen als Plain Text zurückgeben
        /// </summary>
        public static string GetGroupText(DocumentGroup group)
        {
            return StringUtility.CleanAndJoinStringList(group.Paragraphs.Select(p => p.InnerText).ToList(), Environment.NewLine);
        }


        /// <summary>
        /// Gruppen von Absätzen in einem Word-Dokument ermitteln
        /// </summary>
        public static List<DocumentGroup> GetDocumentGroups(WordprocessingDocument document, DocumentGroupOptions options)
        {
            List<DocumentGroup> groups = [];

            if (document == null ||
                document.MainDocumentPart == null ||
                document.MainDocumentPart.Document == null ||
                document.MainDocumentPart.Document.Body == null)
            {
                return groups;
            }

            var body = document.MainDocumentPart.Document.Body;
            var documentParagraphs = body.Elements<Paragraph>().ToList();
            string? previousStyleId = null;
            DocumentGroup currentGroup = new();
            string? language = GetDocumentLanguage(document);

            for (int i = 0; i < documentParagraphs.Count; i++)
            {
                Paragraph currentParagraph = documentParagraphs[i];
                string currentStyleId = currentParagraph.ParagraphProperties?.ParagraphStyleId?.Val?.Value ?? "";
                string styleNameEn = GetStyleNameEn(document, currentStyleId) ?? "";
                string styleName = GetStyleName(currentStyleId) ?? "";

                // Überprüfen, ob der Absatz dem akutellen Stil entspricht
                // Dann so lange Absätze sammeln, bis ein Absatz ohne den Stil 'Sereno' gefunden wird

                if (currentStyleId != previousStyleId)
                {
                    // Vorherige Gruppe speichern
                    DocumentGroup previousGroup = currentGroup;

                    // Neue Gruppe erstellen
                    // Absatz verketten, vorherige Gruppe
                    currentGroup = new()
                    {
                        StyleId = currentStyleId,
                        StyleName = styleName,
                        StyleNameEn = styleNameEn,
                        PreviousGroup = previousGroup
                    };

                    groups.Add(currentGroup);

                    // Absätze verketten, anschließende Gruppe
                    previousGroup.NextGroup = currentGroup;
                }

                currentGroup.Paragraphs.Add(currentParagraph);

                previousStyleId = currentStyleId;
            }

            if (!String.IsNullOrWhiteSpace(options.ParagraphStyleFilter))
            {
                groups = groups.Where(obj => StringUtility.MatchesWildCardPattern(obj.StyleId, options.ParagraphStyleFilter)).ToList();
            }

            if (options.ExtractInnerText)
            {
                foreach (DocumentGroup group in groups)
                {
                    group.InnerText = GetGroupText(group);
                }
            }   

            return groups;
        }


        /// <summary>
        /// Holt die Dokument-Sprache aus settings.xml
        /// </summary>
        public static string GetDocumentLanguage(WordprocessingDocument document)
        {
            var settings = document.MainDocumentPart?.DocumentSettingsPart?.Settings;
            var languages = settings?.Elements<Languages>();
            var langElement = languages?.FirstOrDefault();

            return langElement?.Val?.Value ?? "de-DE"; // Standard: Englisch
        }

        /// <summary>
        /// Holt den Stilnamen aus styles.xml oder StylesWithEffects.xml
        /// </summary>
        public static string? GetStyleNameEn(WordprocessingDocument document, string? styleId)
        {
            if (string.IsNullOrEmpty(styleId))
                return null;

            var styles = document.MainDocumentPart?.StyleDefinitionsPart?.Styles ??
                         document.MainDocumentPart?.StylesWithEffectsPart?.Styles;

            var style = styles?.Elements<Style>().FirstOrDefault(s => s.StyleId == styleId);
            return style?.StyleName?.Val; // Gibt z. B. "Heading 1" zurück
        }


        /// <summary>
        /// Mapped englische Stilnamen auf deutsche
        /// </summary>
        public static string? GetStyleName(string? styleid)
        {
            if (string.IsNullOrEmpty(styleid))
                return null;

            if (styleTranslations.TryGetValue(styleid, out var styleName))
            {
                return styleName;
            }

            return styleid;
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
        /// Liefert alle Dokument-Stile
        /// </summary>
        public static List<(string StyleId, string StyleName)> GetStylesFromDocument(WordprocessingDocument document)
        {
            List<(string StyleId, string StyleName)> styles = new List<(string StyleId, string StyleName)>();

            if (document != null &&
                document.MainDocumentPart != null &&
                document.MainDocumentPart.StyleDefinitionsPart != null &&
                document.MainDocumentPart.StyleDefinitionsPart.Styles != null)
            {
                StylesPart stylesPart = document.MainDocumentPart.StyleDefinitionsPart;
                foreach (Style style in stylesPart.Styles.Elements<Style>())
                {
                    if (style != null &&
                        style.Type != null)
                    {
                        if (style.Type == StyleValues.Paragraph)
                        {
                            string? styleId = styleId = style.StyleId;
                            string? styleName = style.StyleName?.Val?.Value;

                            if (styleId != null && styleName != null)
                            {
                                styles.Add((styleId, styleName));
                                styles.Add((styleId, styleName));
                            }
                        }
                    }
                }
            }

            return styles;
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
            if(options?.Style != null)
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
            List<ColumnOption> columnOptions = new List<ColumnOption>();


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
