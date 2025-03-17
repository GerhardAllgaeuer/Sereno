using Sereno.Office.Word.SimpleStructure;
using Sereno.Utilities;
using System;
using System.Collections.Generic;
using System.IO;

namespace Sereno.Office.Word.Word.SimpleStructure.Converter
{
    public class HtmlConverter : ConverterBase
    {

        public HtmlConverter()
        {
            this.Options = new HtmlConverterOptions();
        }

        public HtmlConverter(HtmlConverterOptions options)
        {
            this.Options = options;
        }
        HtmlConverterOptions Options { get; set; }

        DirectoryInfo templateDirectory;

        public string HtmlDocument { get; set; }
        string htmlDocument = "";

        public string HtmlBody { get; set; }
        string htmlBody = "";



        /// <summary>
        /// Generierte Styles
        /// </summary>
        public string Styles { get; set; }



        protected override void Init()
        {
            string directory = $@"{CodeUtility.GetSolutionDirectory()}\Sereno.Office.Word\Word\SimpleStructure\Converter\Templates\Html";
            this.templateDirectory = new DirectoryInfo(directory);

            htmlDocument = File.ReadAllText($@"{templateDirectory.FullName}\template.html");

            PrepareImagePaths();
        }



        protected override void Finish()
        {
            this.HtmlDocument = htmlDocument.Replace("{{Content}}", htmlBody);
            this.HtmlBody = this.htmlBody;
        }


        public void SaveFiles()
        {
            if (this.Options.ExportRootDirectory == null)
                throw new Exception("Export Directory not set");

            DirectoryInfo fileExportDirectory = new DirectoryInfo(Path.Combine(this.Options.ExportRootDirectory.FullName, this.Options.RelativeImageDirectory));

            if (this.Files.Count > 0)
            {
                DirectoryUtility.EnsureEmptyDirectory(fileExportDirectory);

                foreach (var file in this.Files)
                {
                    string imagePath = Path.Combine(fileExportDirectory.FullName, $"{file.Key}");
                    File.WriteAllBytes(imagePath, file.Value);
                }
            }
            else
            {
                if (fileExportDirectory.Exists)
                {
                    fileExportDirectory.Delete();
                }
            }

        }




        public void SaveStyleSheet()
        {
            if (this.Options.ExportRootDirectory == null)
                throw new Exception("Export Directory not set");

            // css kopieren
            FileInfo cssTemplate = new FileInfo($@"{templateDirectory.FullName}\styles.css");
            DirectoryInfo cssTargetDirectory = this.Options.ExportRootDirectory;

            DirectoryUtility.EnsureDirectory(cssTargetDirectory);

            string targetFilePath = Path.Combine(cssTargetDirectory.FullName, this.Options.CssFileName);

            File.Copy(cssTemplate.FullName, targetFilePath, true);
        }




        public void SaveDocument()
        {
            if (this.Options.ExportRootDirectory == null)
                throw new Exception("Export Directory not set");

            FileInfo file = new FileInfo(Path.Combine(this.Options.ExportRootDirectory.FullName, this.Options.HtmlFileName));

            DirectoryUtility.EnsureDirectory(file.Directory);

            File.WriteAllText(file.FullName, this.HtmlDocument);
        }

        private void PrepareImagePaths()
        {

            if (this.Options.RelativeImageDirectory == null)
                this.Options.RelativeImageDirectory = "";

            if (this.Options.RelativeImageDirectory.StartsWith(@"\"))
            {
                this.Options.RelativeImageDirectory = this.Options.RelativeImageDirectory.Substring(1);
            }

            if (this.Options.RelativeImageDirectory.EndsWith(@"\"))
            {
                this.Options.RelativeImageDirectory = this.Options.RelativeImageDirectory.Substring(0, this.Options.RelativeImageDirectory.Length - 1);
            }



            if (this.Options.RelativeImageHtmlDirectory == null)
                this.Options.RelativeImageHtmlDirectory = "";

            if (this.Options.RelativeImageHtmlDirectory.StartsWith("/"))
            {
                this.Options.RelativeImageHtmlDirectory = this.Options.RelativeImageHtmlDirectory.Substring(1);
            }

            if (this.Options.RelativeImageHtmlDirectory.EndsWith("/"))
            {
                this.Options.RelativeImageHtmlDirectory = this.Options.RelativeImageHtmlDirectory.Substring(0, this.Options.RelativeImageHtmlDirectory.Length - 1);
            }


            if (!String.IsNullOrEmpty(this.Options.RelativeImageHtmlDirectory))
                this.Options.RelativeImageHtmlDirectory = $"{this.Options.RelativeImageHtmlDirectory}/";

        }


        protected override void ProcessGroup(DocumentGroup group)
        {
            int identation = 1;

            if (group is ParagraphGroup paragraphGroup)
            {
                if (paragraphGroup.StyleNameEn == "Title")
                {
                    string title = System.Net.WebUtility.HtmlEncode(paragraphGroup.PlainText);
                    htmlDocument = htmlDocument.Replace("{{Title}}", title);
                }

                else if (paragraphGroup.StyleNameEn == "heading 1")
                {
                    AddToContent("<h1>{{Content}}</h1>", paragraphGroup.PlainText, identation);
                }
                else if (paragraphGroup.StyleNameEn == "heading 2")
                {
                    AddToContent("<h2>{{Content}}</h2>", paragraphGroup.PlainText, identation);
                }
                else if (paragraphGroup.StyleNameEn == "heading 3")
                {
                    AddToContent("<h3>{{Content}}</h3>", paragraphGroup.PlainText, identation);
                }
                else if (paragraphGroup.StyleNameEn == "List Paragraph")
                {
                    AddToContent("<h3>{{Content}}</h3>", paragraphGroup.PlainText, identation);
                }
                else
                {
                    AddToContent("<p>{{Content}}</p>", paragraphGroup.PlainText, identation);
                }
            }
            else if (group is ImageGroup imagegroup)
            {
                AddImagesToContent(imagegroup);
            }
            else if (group is ListParagraphGroup listParagraphGroup)
            {
                AddListParagraphsToContent(listParagraphGroup.ListParagraphs, identation);
            }
            else if (group is TableGroup tableGroup)
            {
                AddTableToContent(tableGroup.TableInfo, identation);
            }
        }

        private void AddImagesToContent(ImageGroup imagegroup)
        {
            // Save images first
            foreach (var image in imagegroup.Images)
            {
                string imageFileName = $"{image.ImageName}.png";
                this.Files.Add(imageFileName, image.Data);
            }

            if (imagegroup.Images.Count > 1)
            {
                // Container für mehrere Bilder nebeneinander
                AddToContent("<div class=\"image-container\">", "", 1);

                foreach (var image in imagegroup.Images)
                {
                    AddToContent("<div class=\"image-wrapper\">", "", 2);
                    AddToContent($"<img src=\"{{{{Content}}}}\" width=\"{image.PixelWidth}\" height=\"{image.PixelHeight}\" alt=\"\" style=\"max-width: 100%; height: auto;\" />",
                        $"{this.Options.RelativeImageHtmlDirectory}{image.ImageName}.png", 3);
                    AddToContent("</div>", "", 2);
                }

                AddToContent("</div>", "", 1);
            }
            else if (imagegroup.Images.Count == 1)
            {
                var image = imagegroup.Images[0];
                // Einzelnes Bild in einem Paragraph
                AddToContent("<p class=\"image-paragraph\">", "", 1);
                AddToContent($"<img src=\"{{{{Content}}}}\" width=\"{image.PixelWidth}\" height=\"{image.PixelHeight}\" alt=\"\" style=\"max-width: 100%; height: auto;\" />",
                    $"{this.Options.RelativeImageHtmlDirectory}{image.ImageName}.png", 2);
                AddToContent("</p>", "", 1);
            }
        }

        private void AddListParagraphsToContent(List<ListParagraph> listParagraphs, int identation)
        {
            if (listParagraphs.Count > 0)
            {
                // Prüfe, ob es sich um eine nummerierte Liste handelt
                bool isNumbered = listParagraphs[0].IsNumbered;
                string listTag = isNumbered ? "ol" : "ul";

                AddToContent($"<{listTag}>", "", identation);

                foreach (ListParagraph listParagraph in listParagraphs)
                {
                    if (listParagraph.Children.Count == 0)
                    {
                        AddToContent("<li>{{Content}}</li>", listParagraph.InnerText, identation + 1);
                    }
                    else
                    {
                        AddToContent("<li>{{Content}}", listParagraph.InnerText, identation + 1, false);
                        htmlBody += Environment.NewLine;
                        AddListParagraphsToContent(listParagraph.Children, identation + 2);
                        AddToContent("</li>", "", identation + 1);
                    }
                }

                AddToContent($"</{listTag}>", "", identation);
            }
        }

        private void AddTableToContent(TableInfo tableInfo, int identation)
        {
            AddToContent("<table>", "", identation);

            // Spaltenbreiten definieren
            AddToContent("<colgroup>", "", identation + 1);
            foreach (ColumnInfo column in tableInfo.Columns)
            {
                // Umrechnung von cm in Pixel (ungefähr 37.8 Pixel pro cm)
                int widthInPixels = (int)(column.ColumnWidth * 37.8);
                AddToContent($"<col style=\"width: {widthInPixels}px\">", "", identation + 2);
            }
            AddToContent("</colgroup>", "", identation + 1);

            // Header-Zeile erstellen, falls vorhanden
            if (tableInfo.HasHeader)
            {
                AddToContent("<thead>", "", identation + 1);
                AddToContent("<tr>", "", identation + 2);

                foreach (ColumnInfo column in tableInfo.Columns)
                {
                    AddToContent("<th>{{Content}}</th>", column.ColumnName, identation + 3);
                }

                AddToContent("</tr>", "", identation + 2);
                AddToContent("</thead>", "", identation + 1);
            }

            // Tabellen-Body erstellen
            AddToContent("<tbody>", "", identation + 1);

            for (int row = 0; row < tableInfo.Data.Rows.Count; row++)
            {
                AddToContent("<tr>", "", identation + 2);

                for (int col = 0; col < tableInfo.Columns.Count; col++)
                {
                    string cellValue = tableInfo.Data.Rows[row][col]?.ToString() ?? "";
                    AddToContent("<td>{{Content}}</td>", cellValue, identation + 3);
                }

                AddToContent("</tr>", "", identation + 2);
            }

            AddToContent("</tbody>", "", identation + 1);
            AddToContent("</table>", "", identation);
        }

        private void AddToContent(string template, string replacementText, int identation, bool withNewLine = true)
        {
            string text = template.Replace("{{Content}}", System.Net.WebUtility.HtmlEncode(replacementText));

            text = "".PadLeft(identation * 4) + text;

            if (withNewLine)
                text += Environment.NewLine;

            htmlBody += text;
        }
    }
}
