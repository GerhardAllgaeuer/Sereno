using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Sereno.Office.Word.SimpleStructure;
using System.Collections.Generic;

namespace Sereno.Office.Word.Word.SimpleStructure
{
    public class ListParagraphGroupUtility
    {
        /// <summary>
        /// Zusammenhängende Auflistungen zusammenführen
        /// </summary>
        public static void CompressListParagraphs(List<DocumentGroup> groups)
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
        }


        public static ListParagraphGroup ProcessListParagraph(Paragraph paragraph, WordprocessingDocument document, DocumentGroupOptions options)
        {
            string currentStyleId = paragraph.ParagraphProperties?.ParagraphStyleId?.Val?.Value ?? "";
            string styleNameEn = ParagraphGroupUtility.GetStyleNameEn(document, currentStyleId) ?? "";
            string styleName = ParagraphGroupUtility.GetStyleName(currentStyleId) ?? "";

            ParagraphProperties paragraphProperties = paragraph.GetFirstChild<ParagraphProperties>();
            NumberingProperties numbering = paragraphProperties.GetFirstChild<NumberingProperties>();

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
    }
}
