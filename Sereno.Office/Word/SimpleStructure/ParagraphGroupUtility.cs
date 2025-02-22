using Sereno.Utilities;

namespace Sereno.Office.Word.SimpleStructure
{
    public class ParagraphGroupUtility
    {
        /// <summary>
        /// Text aus den Paragraphen als Plain Text zurückgeben
        /// </summary>
        public static void ExtractParagraphGroupText(ParagraphGroup paragraph)
        {
            paragraph.InnerText = StringUtility.CleanAndJoinStringList(paragraph.Paragraphs.Select(p => p.InnerText).ToList(), Environment.NewLine);
        }


        /// <summary>
        /// Text aus den Paragraphen als Plain Text zurückgeben
        /// </summary>
        public static void FilterParagraphs(List<DocumentGroup> groups, DocumentGroupOptions options)
        {
            // Stil-Filter anwenden
            if (!String.IsNullOrWhiteSpace(options.ParagraphStyleFilter))
            {
                groups.RemoveAll(obj => !StringUtility.MatchesWildCardPattern(obj.StyleId, options.ParagraphStyleFilter));
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
            ParagraphGroup? previousGroup = null;
            List<DocumentGroup> groupsToRemove = [];

            List<ParagraphGroup> paragraphs = groups.OfType<ParagraphGroup>().ToList();

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
    }
}
