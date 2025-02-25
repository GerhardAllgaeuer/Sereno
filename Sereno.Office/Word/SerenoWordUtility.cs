using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml;
using Sereno.Office.Word.SimpleStructure;
using System.Collections.Generic;
using System;

namespace Sereno.Office.Word
{
    public class SerenoWordUtility
    {

        /// <summary>
        /// Sereno-Kommandos in einem Word-Dokument verarbeiten
        /// </summary>
        public static void ProcessDocument(WordprocessingDocument document, Action<OpenXmlElement, string> action)
        {

            DocumentGroupOptions options = new DocumentGroupOptions()
            {
                ParagraphStyleFilter = "Sereno",
            };

            List<DocumentGroup> groups = DocumentGroupUtility.GetDocumentGroups(document, options);

            if (document == null ||
                document.MainDocumentPart == null ||
                document.MainDocumentPart.Document == null ||
                document.MainDocumentPart.Document.Body == null)
            {
                return;
            }
        }
    }
}
