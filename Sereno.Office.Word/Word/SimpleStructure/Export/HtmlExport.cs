using DocumentFormat.OpenXml.Office2016.Drawing.Command;
using Sereno.Office.Word.SimpleStructure;
using Sereno.Utilities;
using System;
using System.IO;

namespace Sereno.Office.Word.Word.SimpleStructure.Export
{
    public class HtmlExport : ExportBase
    {
        DirectoryInfo templateDirectory;

        string htmlContent = "";
        string content = "";

        protected override void Init()
        {
            string directory = $@"{CodeUtility.GetSolutionDirectory()}\Sereno.Office.Word\Word\SimpleStructure\Export\Templates\Html";
            this.templateDirectory = new DirectoryInfo(directory);

            htmlContent = File.ReadAllText($@"{templateDirectory.FullName}\template.html");
        }

        protected override void Finish()
        {
            // css kopieren
            FileInfo cssTemplate = new FileInfo($@"{templateDirectory.FullName}\styles.css");
            FileInfo cssTarget = new FileInfo($@"{this.Options.ExportDirectory}\{cssTemplate.Name}");
            File.Copy(cssTemplate.FullName, cssTarget.FullName, true);

            htmlContent = htmlContent.Replace("{{Content}}", content);

            SaveHtmlFile();
        }

        private void SaveHtmlFile()
        {
            FileInfo file = new FileInfo($@"{this.Options.ExportDirectory}\document.html");
            File.WriteAllText(file.FullName, htmlContent);
        }

        protected override void ProcessGroup(DocumentGroup group)
        {
            if (group is ParagraphGroup paragraph)
            {
                if (paragraph.StyleNameEn == "Title")
                {
                    string title = System.Net.WebUtility.HtmlEncode(paragraph.InnerText);
                    htmlContent = htmlContent.Replace("{{Title}}", title);
                }

                else if (paragraph.StyleNameEn == "heading 1")
                {
                    AddToContent("<h1>{{Content}}</h1>", paragraph.InnerText);
                }
                else if (paragraph.StyleNameEn == "heading 2")
                {
                    AddToContent("<h2>{{Content}}</h2>", paragraph.InnerText);
                }
                else if (paragraph.StyleNameEn == "heading 3")
                {
                    AddToContent("<h3>{{Content}}</h3>", paragraph.InnerText);
                }
            }
        }

        private void AddToContent(string template, string replacementText)
        {
            string text = template.Replace("{{Content}}", System.Net.WebUtility.HtmlEncode(replacementText));
            text += Environment.NewLine;

            content += text;
        }
    }
}
