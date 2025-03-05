using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Sereno.Office.Word.SimpleStructure;
using System.Collections.Generic;
using System.Linq;

namespace Sereno.Office.Word.Word.SimpleStructure
{
    public class ListParagraphGroupUtility
    {
        /// <summary>
        /// Zusammenhängende Auflistungen zusammenführen
        /// </summary>
        public static void CompressListParagraphs(List<DocumentGroup> groups, WordprocessingDocument document)
        {
            ListParagraphGroup previousGroup = null;
            List<DocumentGroup> groupsToRemove = new List<DocumentGroup>();

            foreach (DocumentGroup group in groups)
            {
                if (group is ListParagraphGroup currentGroup)
                {
                    if (previousGroup != null)
                    {
                        previousGroup.Paragraphs.AddRange(currentGroup.Paragraphs);
                        groupsToRemove.Add(group);
                    }
                    else
                    {
                        previousGroup = currentGroup;
                    }
                }
                else
                {
                    // wenn eine andere Gruppe dazwischen ist, dann wird nicht zusammengefügt
                    previousGroup = null;
                }
            }

            foreach (DocumentGroup group in groupsToRemove)
            {
                groups.Remove(group);
            }

            BuildListParagraphHierarchy(groups, document.MainDocumentPart);
        }

        private static void BuildListParagraphHierarchy(List<DocumentGroup> groups, MainDocumentPart mainPart)
        {
            List<ListParagraphGroup> paragraphs = groups.OfType<ListParagraphGroup>()
                                                        .ToList();

            if (!paragraphs.Any()) return;

            foreach (ListParagraphGroup paragraphGroup in paragraphs)
            {
                var listParagraphs = GetListParagraphs(paragraphGroup, mainPart);
                paragraphGroup.ListParagraphs = BuildHierarchy(listParagraphs, 0, 0);
            }
        }

        private static List<ListParagraph> GetListParagraphs(ListParagraphGroup paragraphGroup, MainDocumentPart mainPart)
        {
            List<ListParagraph> listItems = new List<ListParagraph>();

            foreach (var paragraph in paragraphGroup.Paragraphs)
            {
                var paraProps = paragraph.GetFirstChild<ParagraphProperties>();
                if (paraProps == null) continue;

                var numberingProp = paraProps.GetFirstChild<NumberingProperties>();
                if (numberingProp == null) continue;

                // Bestimme die Einrückungsebene (Standard 0, falls nicht vorhanden)
                int indentLevel = numberingProp.GetFirstChild<NumberingLevelReference>()?.Val?.Value ?? 0;
                
                // Hole die NumId für die Unterscheidung zwischen Aufzählung und Nummerierung
                var numId = numberingProp.GetFirstChild<NumberingId>()?.Val?.Value ?? 0;

                // Bestimme den Nummerierungstyp aus dem Dokument
                string numberingFormat = GetNumberingFormat(mainPart.Document, numId, indentLevel);
                bool isNumbered = numberingFormat != null && numberingFormat != "bullet";

                listItems.Add(new ListParagraph
                {
                    InnerText = paragraph.InnerText,
                    IndentLevel = indentLevel,
                    Children = new List<ListParagraph>(),
                    Paragraph = paragraph,
                    NumberingId = numId,
                    IsNumbered = isNumbered
                });
            }

            return listItems;
        }

        private static List<ListParagraph> BuildHierarchy(List<ListParagraph> paragraphs, int startIndex, int currentIndent)
        {
            List<ListParagraph> result = new List<ListParagraph>();

            for (int i = startIndex; i < paragraphs.Count; i++)
            {
                if (paragraphs[i].IndentLevel == currentIndent)
                {
                    List<ListParagraph> children = BuildHierarchy(paragraphs, i + 1, currentIndent + 1);
                    paragraphs[i].Children.AddRange(children);
                    result.Add(paragraphs[i]);
                }
                else if (paragraphs[i].IndentLevel < currentIndent)
                {
                    return result;
                }
            }
            return result;
        }


        public static ListParagraphGroup ProcessListParagraph(Paragraph paragraph, WordprocessingDocument document, DocumentGroupOptions options)
        {
            string currentStyleId = paragraph.ParagraphProperties?.ParagraphStyleId?.Val?.Value ?? "";
            string styleNameEn = ParagraphGroupUtility.GetStyleNameEn(document, currentStyleId) ?? "";
            string styleName = ParagraphGroupUtility.GetStyleName(currentStyleId) ?? "";

            ListParagraphGroup result = new ListParagraphGroup()
            {
                StyleId = currentStyleId,
                StyleName = styleName,
                StyleNameEn = styleNameEn,
            };

            result.Paragraphs.Add(paragraph);

            return result;
        }

        public static bool IsListParagraph(Paragraph paragraph)
        {
            bool result = false;

            ParagraphProperties paragraphProperties = paragraph.GetFirstChild<ParagraphProperties>();
            if (paragraphProperties != null)
            {
                NumberingProperties numbering = paragraphProperties.GetFirstChild<NumberingProperties>();
                if (numbering != null)
                {
                    result = true;
                }
            }

            return result;
        }

        // Zusätzliche Methode um Nummerierungstyp zu bestimmen
        private static string GetNumberingFormat(Document doc, int numId, int level)
        {
            var numberingPart = doc.MainDocumentPart.NumberingDefinitionsPart;
            if (numberingPart == null) return null;

            var numbering = numberingPart.Numbering;
            var abstractNumId = numbering
                .Elements<NumberingInstance>()
                .FirstOrDefault(ni => ni.NumberID == numId)?
                .AbstractNumId?.Val;

            if (abstractNumId == null) return null;

            var abstractNum = numbering
                .Elements<AbstractNum>()
                .FirstOrDefault(an => an.AbstractNumberId == abstractNumId);

            var levelFormat = abstractNum?
                .Elements<Level>()
                .FirstOrDefault(l => l.LevelIndex == level)?
                .NumberingFormat?.Val;

            return levelFormat?.ToString();
        }
    }

}
