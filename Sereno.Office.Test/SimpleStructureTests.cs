using DocumentFormat.OpenXml.Packaging;
using FluentAssertions;
using Sereno.Office.Word;
using Sereno.Office.Word.SimpleStructure;

namespace Sereno.Office.Test
{
    [TestClass]
    public sealed class SimpleStructureTests
    {
        [TestMethod]
        public void Read_All_Paragraphs()
        {
            string filePath = @"D:\Projekte\Privat\Sereno\Sereno.Office.Test\Templates\Test_0001.docx";

            using (WordprocessingDocument? document = WordUtility.OpenWordDocument(filePath))
            {
                if (document != null)
                {
                    List<ParagraphGroup> paragraphs = DocumentGroupUtility.GetDocumentGroups(document)
                                                                .OfType<ParagraphGroup>()
                                                                .Where(x => !String.IsNullOrWhiteSpace(x.InnerText))
                                                                .ToList();

                    var expectedGroups = new[]
                    {
                        new { StyleId = "Titel", StyleName = "Titel", StyleNameEn = "Title", InnerText = "Test Titel" },
                        new { StyleId = "berschrift1", StyleName = "Überschrift 1", StyleNameEn = "heading 1", InnerText = "Test Ü1" },
                        new { StyleId = "berschrift2", StyleName = "Überschrift 2", StyleNameEn = "heading 2", InnerText = "Test Ü2" },
                        new { StyleId = "berschrift3", StyleName = "Überschrift 3", StyleNameEn = "heading 3", InnerText = "Test Ü3" },
                        new { StyleId = "", StyleName = "", StyleNameEn = "", InnerText = "Standard" },
                        new { StyleId = "", StyleName = "", StyleNameEn = "", InnerText = "https://test.com" },
                    };

                    paragraphs.Should().BeEquivalentTo(expectedGroups);
                }
            }
        }
    }
}
