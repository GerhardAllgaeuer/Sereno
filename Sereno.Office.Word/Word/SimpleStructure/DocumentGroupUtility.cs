﻿using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Wordprocessing;
using System.Collections.Generic;
using System.Linq;
using Sereno.Office.Word.Word.SimpleStructure;

namespace Sereno.Office.Word.SimpleStructure
{
    public class DocumentGroupUtility
    {


        /// <summary>
        /// Gruppen von Absätzen in einem Word-Dokument ermitteln
        /// </summary>
        public static List<DocumentGroup> GetDocumentGroups(WordprocessingDocument document, DocumentGroupOptions options = null)
        {
            if (options == null)
            {
                options = new DocumentGroupOptions();
            }

            List<DocumentGroup> groups = new List<DocumentGroup>();

            if (document == null ||
                document.MainDocumentPart == null ||
                document.MainDocumentPart.Document == null ||
                document.MainDocumentPart.Document.Body == null)
            {
                return groups;
            }

            var body = document.MainDocumentPart.Document.Body;

            List<OpenXmlElement> elements = body.Elements().Where(e => e is Paragraph ||
                                                      e is Table)
                                          .ToList();

            DocumentGroup currentGroup = null;
            DocumentGroup previousGroup = null;

            foreach (var element in elements)
            {
                if (element is Paragraph paragraph)
                {
                    if (ListParagraphGroupUtility.IsListParagraph(paragraph))
                    {
                        currentGroup = ListParagraphGroupUtility.ProcessListParagraph(paragraph, document, options);
                    }
                    else if (ImageGroupUtility.ParagraphContainsAnImage(paragraph))
                    {
                        currentGroup = ImageGroupUtility.ProcessParagraph(paragraph, document, options);
                    }
                    else
                    {
                        currentGroup = ParagraphGroupUtility.ProcessParagraph(paragraph, document, options);
                    }

                }
                else if (element is Table table)
                {
                    currentGroup = TableGroupUtility.ProcessTable(table);
                }

                if (currentGroup != null)
                {
                    groups.Add(currentGroup);
                    currentGroup.PreviousGroup = previousGroup;
                }

                if (previousGroup != null)
                {
                    previousGroup.NextGroup = currentGroup;
                }


                previousGroup = currentGroup;
            }


            ImageGroupUtility.PrepareImages(groups);
            ListParagraphGroupUtility.CompressListParagraphs(groups, document);
            ParagraphGroupUtility.CompressParagraphsByStyle(groups);
            ParagraphGroupUtility.ExtractParagraphGroupText(groups);

            ParagraphGroupUtility.FilterParagraphs(groups, options);



            return groups;
        }



    }
}
