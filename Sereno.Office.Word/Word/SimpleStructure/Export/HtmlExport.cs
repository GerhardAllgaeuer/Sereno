using Sereno.Office.Word.SimpleStructure;
using Sereno.Utilities;
using System;
using System.Collections.Generic;
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

            if (!this.Options.ExportDirectory.Exists)
                this.Options.ExportDirectory.Create();

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
            int identation = 1;

            if (group is ParagraphGroup paragraphGroup)
            {
                if (paragraphGroup.StyleNameEn == "Title")
                {
                    string title = System.Net.WebUtility.HtmlEncode(paragraphGroup.InnerText);
                    htmlContent = htmlContent.Replace("{{Title}}", title);
                }

                else if (paragraphGroup.StyleNameEn == "heading 1")
                {
                    AddToContent("<h1>{{Content}}</h1>", paragraphGroup.InnerText, identation);
                }
                else if (paragraphGroup.StyleNameEn == "heading 2")
                {
                    AddToContent("<h2>{{Content}}</h2>", paragraphGroup.InnerText, identation);
                }
                else if (paragraphGroup.StyleNameEn == "heading 3")
                {
                    AddToContent("<h3>{{Content}}</h3>", paragraphGroup.InnerText, identation);
                }
                else if (paragraphGroup.StyleNameEn == "List Paragraph")
                {
                    AddToContent("<h3>{{Content}}</h3>", paragraphGroup.InnerText, identation);
                }
                else
                {
                    AddToContent("<p>{{Content}}</p>", paragraphGroup.InnerText, identation);
                }
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
                        content += Environment.NewLine;
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

            content += text;
        }
    }
}
