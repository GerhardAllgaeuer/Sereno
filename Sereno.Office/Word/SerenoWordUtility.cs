using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Wordprocessing;
using Sereno.Utilities;

namespace Sereno.Office.Word
{
    public class SerenoWordUtility
    {


        /// <summary>
        /// Sereno-Kommandos in einem Word-Dokument verarbeiten
        /// </summary>
        public static void ProcessDocument(WordprocessingDocument document, Action<OpenXmlElement?, string?> action)
        {

            DocumentGroupOptions options = new()
            {
                ParagraphStyleFilter = "Sereno",
                ExtractInnerText = true,
            };

            List<DocumentGroup> groups = WordUtility.GetDocumentGroups(document, options);

            if (document == null ||
                document.MainDocumentPart == null ||
                document.MainDocumentPart.Document == null ||
                document.MainDocumentPart.Document.Body == null)
            {
                return;
            }

            foreach (DocumentGroup group in groups)
            {
                action(group?.NextGroup?.Paragraphs.FirstOrDefault(), group!.InnerText); // Rufe die Aktion auf
            }

        }
    }
}
