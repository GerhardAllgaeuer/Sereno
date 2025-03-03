using DocumentFormat.OpenXml.Packaging;
using FluentAssertions;
using Sereno.Office.Word;
using Sereno.Office.Word.SimpleStructure;
using Sereno.Test;
using Sereno.Test.Database;
using Sereno.Utilities;
using System.Data;

namespace Sereno.Office.Test.SimpleStructure
{
    [TestClass]
    public sealed class ReaderTests
    {
        [TestMethod]
        public void Read_All_Paragraphs()
        {
            string filePath = $@"{CodeUtility.GetProjectRoot()}\Sereno.Office.Test\Templates\Test_0001.docx";

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


        [TestMethod]
        public void Read_Table_Without_Header()
        {
            string filePath = $@"{CodeUtility.GetProjectRoot()}\Sereno.Office.Test\Templates\Test_0001.docx";

            using (WordprocessingDocument document = WordUtility.OpenWordDocument(filePath))
            {
                var table = DocumentGroupUtility.GetDocumentGroups(document)
                                              .OfType<TableGroup>()
                                              .First();

                var options = new TableInfoOptions { DetermineHeaderRow = true };
                var tableInfo = TableGroupUtility.GetTableInfo(table, options);

                tableInfo.Should().BeEquivalentTo(new
                {
                    HasHeader = false,
                    Columns = new[]
                    {
                        new { ColumnName = "Column 0" },
                        new { ColumnName = "Column 1" },
                    },
                });

                var expectedData = new[]
                {
                    new { Column1 = "Zeile 1, Spalte 1", Column2 = "Zeile 1, Spalte 2" },
                    new { Column1 = "Zeile 2, Spalte 1", Column2 = "Zeile 2, Spalte 2" }
                };

                tableInfo.Data.ShouldBeEquivalentTo(expectedData);
            }
        }

        [TestMethod]
        public void Read_Table_With_Header_Auto_Detect()
        {
            string filePath = $@"{CodeUtility.GetProjectRoot()}\Sereno.Office.Test\Templates\Test_0001.docx";

            using (WordprocessingDocument document = WordUtility.OpenWordDocument(filePath))
            {
                var table = DocumentGroupUtility.GetDocumentGroups(document)
                                              .OfType<TableGroup>()
                                              .Skip(1)
                                              .First();

                var options = new TableInfoOptions { DetermineHeaderRow = true };
                var tableInfo = TableGroupUtility.GetTableInfo(table, options);

                tableInfo.Should().BeEquivalentTo(new
                {
                    HasHeader = true,
                    Columns = new[]
                    {
                        new { ColumnName = "Column 1" },
                        new { ColumnName = "Column 2" },
                    },
                });

                var expectedData = new[]
                {
                    new { Column1 = "Zeile 1, Spalte 1", Column2 = "Zeile 1, Spalte 2" },
                    new { Column1 = "Zeile 2, Spalte 1", Column2 = "Zeile 2, Spalte 2" }
                };

                tableInfo.Data.ShouldBeEquivalentTo(expectedData);
            }
        }

        [TestMethod]
        public void Read_Table_With_Header_Manual_Setting()
        {
            string filePath = $@"{CodeUtility.GetProjectRoot()}\Sereno.Office.Test\Templates\Test_0001.docx";

            using (WordprocessingDocument document = WordUtility.OpenWordDocument(filePath))
            {
                var table = DocumentGroupUtility.GetDocumentGroups(document)
                                              .OfType<TableGroup>()
                                              .Skip(1)
                                              .First();

                var options = new TableInfoOptions 
                { 
                    DetermineHeaderRow = false,
                    HasHeaderRow = true
                };
                var tableInfo = TableGroupUtility.GetTableInfo(table, options);

                tableInfo.Should().BeEquivalentTo(new
                {
                    HasHeader = true,
                    Columns = new[]
                    {
                        new { ColumnName = "Column 1" },
                        new { ColumnName = "Column 2" },
                    },
                });

                var expectedData = new[]
                {
                    new { Column1 = "Zeile 1, Spalte 1", Column2 = "Zeile 1, Spalte 2" },
                    new { Column1 = "Zeile 2, Spalte 1", Column2 = "Zeile 2, Spalte 2" }
                };

                tableInfo.Data.ShouldBeEquivalentTo(expectedData);
            }
        }
    }
}
