using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Sereno.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sereno.Office.Word.SimpleStructure
{
    public class ParagraphGroupUtility
    {


        /// <summary>
        /// Deutsche Styles werden nicht sauber in Deutsch übersetzt, wir helfen mit dem Dictionary nach
        /// </summary>
        private static readonly Dictionary<string, string> styleTranslations = new Dictionary<string, string>()
        {
            { "berschrift1", "Überschrift 1" },
            { "berschrift2", "Überschrift 2" },
            { "berschrift3", "Überschrift 3" },
            { "berschrift4", "Überschrift 4" },
        };



        /// <summary>
        /// Paragraph Gruppen Objekt erstellen
        /// </summary>
        public static ParagraphGroup ProcessParagraph(Paragraph paragraph, WordprocessingDocument document, DocumentGroupOptions options)
        {
            string currentStyleId = paragraph.ParagraphProperties?.ParagraphStyleId?.Val?.Value ?? "";
            string styleNameEn = GetStyleNameEn(document, currentStyleId) ?? "";
            string styleName = GetStyleName(currentStyleId) ?? "";

            ParagraphGroup result = new ParagraphGroup()
            {
                StyleId = currentStyleId,
                StyleName = styleName,
                StyleNameEn = styleNameEn,
            };

            result.Paragraphs.Add(paragraph);

            return result;
        }


        /// <summary>
        /// Text aus den Paragraphen als Plain Text zurückgeben
        /// </summary>
        public static void ExtractParagraphGroupText(ParagraphGroup paragraph)
        {
            paragraph.PlainText = StringUtility.CleanAndJoinStringList(paragraph.Paragraphs.Select(p => p.InnerText).ToList(), Environment.NewLine);
        }


        public static void FilterParagraphs(List<DocumentGroup> groups, DocumentGroupOptions options)
        {
            // Stil-Filter anwenden
            if (!string.IsNullOrWhiteSpace(options.ParagraphStyleFilter))
            {
                groups.RemoveAll(obj => obj is ParagraphGroup pg && !StringUtility.MatchesWildCardPattern(pg.StyleId, options.ParagraphStyleFilter));
            }
        }


        /// <summary>
        /// Texte aus den Paragraphen extrahieren
        /// </summary>
        public static void ExtractParagraphGroupText(List<DocumentGroup> groups)
        {
            List<ParagraphGroup> paragraphs = groups.OfType<ParagraphGroup>().ToList();

            foreach (ParagraphGroup paragraph in paragraphs)
            {
                ExtractParagraphGroupText(paragraph);
            }
        }



        /// <summary>
        /// Paragraphen vom gleich Stil zusammenfügen
        /// </summary>
        public static void CompressParagraphsByStyle(List<DocumentGroup> groups)
        {
            ParagraphGroup previousGroup = null;
            List<DocumentGroup> groupsToRemove = new List<DocumentGroup>();

            foreach (DocumentGroup group in groups)
            {
                if (group is ParagraphGroup currentGroup)
                {
                    if (previousGroup != null)
                    {
                        if (previousGroup.StyleId == currentGroup.StyleId)
                        {
                            previousGroup.Paragraphs.AddRange(currentGroup.Paragraphs);
                            groupsToRemove.Add(group);
                        }
                        else
                        {
                            previousGroup = currentGroup;
                        }
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
                            string styleId = styleId = style.StyleId;
                            string styleName = style.StyleName?.Val?.Value;

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
        /// Holt den Stilnamen aus styles.xml oder StylesWithEffects.xml
        /// </summary>
        public static string GetStyleNameEn(WordprocessingDocument document, string styleId)
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
        public static string GetStyleName(string styleid)
        {
            if (string.IsNullOrEmpty(styleid))
                return null;

            if (styleTranslations.TryGetValue(styleid, out var styleName))
            {
                return styleName;
            }

            return styleid;
        }
    }
}
