using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using FluentAssertions;
using Microsoft.CodeCoverage.Core.Reports.Coverage;
using Sereno.Office.Word;
using System.IO;

namespace Sereno.Office.Test
{
    [TestClass]
    public sealed class Office
    {
        [TestMethod]
        public void Read_All_Paragraph_Types()
        {
            string filePath = @"D:\Projekte\Privat\Sereno\Sereno.Office.Test\Templates\Test_0001.docx";

            using (WordprocessingDocument? wordDocument = WordUtility.OpenWordDocument(filePath))
            {
                if (wordDocument != null)
                {
                    DocumentGroupOptions options = new DocumentGroupOptions()
                    {
                        ExtractInnerText = true,
                    };

                    List<DocumentGroup> groups = WordUtility.GetDocumentGroups(wordDocument, options);

                    List<DocumentGroup> groupsToCompare = groups.Take(2).ToList();

                    var expectedGroups = new[]
                    {
                        new { StyleId = "Titel", StyleName = "Titel", StyleNameEn = "Title" },
                        new { StyleId = "berschrift1", StyleName = "Überschrift 1", StyleNameEn = "heading 1" }
                    };


                    groupsToCompare.Should().BeEquivalentTo(expectedGroups);
                }
            }
        }
    }
}
