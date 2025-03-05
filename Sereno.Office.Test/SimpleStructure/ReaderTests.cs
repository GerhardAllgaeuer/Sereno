using DocumentFormat.OpenXml.Packaging;
using FluentAssertions;
using Sereno.Office.Word;
using Sereno.Office.Word.SimpleStructure;
using Sereno.Test.Database;
using Sereno.Utilities;
using System.Data;

namespace Sereno.Office.Test.SimpleStructure
{
    [TestClass]
    public sealed class ReaderTests
    {
        [TestMethod]
        public void Read_List_Paragraphs()
        {
            string filePath = $@"{CodeUtility.GetProjectRoot()}\Sereno.Office.Test\Documents\List_Paragraphs.docx";

            using (WordprocessingDocument document = WordUtility.OpenWordDocument(filePath))
            {
                List<ListParagraphGroup> paragraphs = DocumentGroupUtility.GetDocumentGroups(document)
                                                            .OfType<ListParagraphGroup>()
                                                            .ToList();

                var expectedGroups = new[]
                {
                    new { StyleNameEn = "List Paragraph" },
                    new { StyleNameEn = "List Paragraph" },
                };
                paragraphs.Should().BeEquivalentTo(expectedGroups);


                // Anzahl Elemente prüfen
                int firstChildrenCount = paragraphs[0].ListParagraphs[0].Children.Count;
                firstChildrenCount.Should().Be(3, $"Auflistung 0 muss 3 Elemente besitzen");

                // Ebene 3 prüfen
                int level3Count = paragraphs[0].ListParagraphs[2].Children[0].Children.Count;
                level3Count.Should().Be(1, $"Auflistung auf Ebene 3 muss 1 Element besitzen");


                // Nummerierung prüfen
                bool isfirstNumbered = paragraphs[0].ListParagraphs[0].IsNumbered;
                isfirstNumbered.Should().Be(false, $"Auflistung 0 ist nicht nummeriert.");

                bool isfirstChildNumbered = paragraphs[0].ListParagraphs[0].Children[0].IsNumbered;
                isfirstChildNumbered.Should().Be(true, $"Unterelemente der Auflistung 0 müssen nummeriert sein.");
            }
        }



        [TestMethod]
        public void Read_All_Paragraphs()
        {
            string filePath = $@"{CodeUtility.GetProjectRoot()}\Sereno.Office.Test\Documents\All_Types.docx";

            using (WordprocessingDocument document = WordUtility.OpenWordDocument(filePath))
            {
                List<ParagraphGroup> paragraphs = DocumentGroupUtility.GetDocumentGroups(document)
                                                            .OfType<ParagraphGroup>()
                                                            .Where(x => !string.IsNullOrWhiteSpace(x.InnerText))
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
