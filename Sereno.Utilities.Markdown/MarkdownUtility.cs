using Markdig;
using Markdig.Syntax;
using System.Text;

namespace Sereno.Utilities
{
    public class MarkdownUtility
    {
        public static string FindSectionText(string markdown, string headerToFind)
        {
            var pipeline = new MarkdownPipelineBuilder().Build();
            var document = Markdown.Parse(markdown, pipeline);
            StringBuilder sectionContent = new StringBuilder();
            bool collectText = false;

            foreach (var block in document)
            {
                if (block is HeadingBlock heading && heading.Level == 1)
                {
                    if (collectText) break; // Stoppe, wenn der nächste Header 1 gefunden wird

                    string? headingText = heading?.Inline?.FirstChild?.ToString();
                    if (headingText != null &&
                        headingText.Equals(headerToFind, StringComparison.OrdinalIgnoreCase))
                    {
                        collectText = true; // Starte die Textsammlung, wenn der gesuchte Header gefunden wird
                    }
                }
                else if (collectText)
                {
                    // Sammle den Text der Sektion
                    if (block is ParagraphBlock paragraph)
                    {
                        sectionContent.AppendLine(paragraph?.Inline?.FirstChild?.ToString());
                    }
                }
            }

            return sectionContent.ToString().Trim();
        }
    }

}

