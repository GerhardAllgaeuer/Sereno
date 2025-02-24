using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System.Text;

namespace Sereno.Office.Word
{
    public class WordExportUtility
    {

        /// <summary>
        /// Exportiert die Struktur eines Word-Dokuments in eine CSV-Datei.
        /// </summary>
        public static void ExportDocumentStructureToCsv(WordprocessingDocument document, string outputCsvPath)
        {
            if (document == null) throw new ArgumentNullException(nameof(document));
            if (string.IsNullOrEmpty(outputCsvPath)) throw new ArgumentException("Output CSV path cannot be null or empty.", nameof(outputCsvPath));

            using (var writer = new StreamWriter(outputCsvPath, false, Encoding.UTF8))
            {
                writer.WriteLine("\"Type\";\"Text\""); // Header

                // Hauptdokumentteil durchlaufen
                if (document.MainDocumentPart?.Document != null)
                {
                    ProcessElement(writer, document.MainDocumentPart.Document, 0);
                }
            }
        }


        /// <summary>
        /// Rekursive Methode zum Verarbeiten eines OpenXmlElements und seiner Kinder.
        /// </summary>
        /// <param name="writer">Der StreamWriter für die CSV-Ausgabe.</param>
        /// <param name="element">Das aktuelle OpenXmlElement.</param>
        /// <param name="indentationLevel">Die aktuelle Einrückungsebene.</param>
        private static void ProcessElement(StreamWriter writer, OpenXmlElement element, int indentationLevel)
        {
            string indentation = new string(' ', indentationLevel * 2); // 2 Leerzeichen pro Ebene
            string type = element.GetType().Name;
            string text = string.Empty;

            // Wenn das Element ein Text-Element ist, Text abrufen
            if (element is Text textElement)
            {
                text = textElement.Text.Replace("\r", "").Replace("\n", "");
            }

            // Wenn das Element ein Feld ist, Text abrufen
            if (element is FieldCode fieldCodeElement)
            {
                text = fieldCodeElement.InnerText.Replace("\r", "").Replace("\n", "");
            }


            // Wenn das Element ein Feld ist, Text abrufen
            if (element is FieldChar fieldCharElement)
            {
                text = fieldCharElement.InnerText.Replace("\r", "").Replace("\n", "");
            }

            if (element is TableProperties tableProperties)
            {
            }

            // Zeile in die CSV schreiben
            writer.WriteLine($"\"{indentation}{type}\";\"{text}\"");

            // Rekursiv durch die Kinder des Elements gehen
            foreach (var child in element.ChildElements)
            {
                ProcessElement(writer, child, indentationLevel + 1);
            }
        }
    }
}
