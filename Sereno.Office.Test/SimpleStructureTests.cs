using DocumentFormat.OpenXml.Packaging;
using FluentAssertions;
using Sereno.Office.Word;
using Sereno.Office.Word.SimpleStructure;
using Sereno.Test;
using Sereno.Test.Database;
using System.Data;

namespace Sereno.Office.Test
{
    [TestClass]
    public sealed class SimpleStructureTests
    {
        [TestMethod]
        public void Read_All_Paragraphs()
        {
            string filePath = $@"{TestUtility.GetProjectRoot()}\Sereno.Office.Test\Templates\Test_0001.docx";

            using (WordprocessingDocument document = WordUtility.OpenWordDocument(filePath))
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


        [TestMethod]
        public void Read_Tables()
        {
            string filePath = $@"{TestUtility.GetProjectRoot()}\Sereno.Office.Test\Templates\Test_0001.docx";

            using (WordprocessingDocument document = WordUtility.OpenWordDocument(filePath))
            {
                List<TableGroup> tables = DocumentGroupUtility.GetDocumentGroups(document)
                                                            .OfType<TableGroup>()
                                                            .ToList();

                // Tabelle ohne Header
                TableGroup table0 = tables[0];
                TableInfoOptions options0 = new TableInfoOptions()
                {
                     DetermineHeaderRow = true,
                };
                TableInfo tableInfo0 = TableGroupUtility.GetTableInfo(table0, options0);

                var expectedInfo0 = new
                {
                    HasHeader = false,
                    Columns = new[]
                    {
                        new { ColumnName = "Column 0" },
                        new { ColumnName = "Column 1" },
                    },
                };

                tableInfo0.Should().BeEquivalentTo(expectedInfo0);


                var expectedData0 = new[]
                {
                    new { Column1 = "Zeile 1, Spalte 1", Column2 = "Zeile 1, Spalte 2" },
                    new { Column1 = "Zeile 2, Spalte 1", Column2 = "Zeile 2, Spalte 2" }
                };

                tableInfo0.Data.ShouldBeEquivalentTo(expectedData0);



                // Tabelle mit Header
                TableGroup table1 = tables[1];
                TableInfoOptions options1 = new TableInfoOptions()
                {
                    DetermineHeaderRow = true,
                }; 
                TableInfo tableInfo1 = TableGroupUtility.GetTableInfo(table1, options1);

                var expectedInfo1 = new
                {
                    HasHeader = true,
                    Columns = new[]
    {
                        new { ColumnName = "Column 1" },
                        new { ColumnName = "Column 2" },
                    },
                };

                tableInfo1.Should().BeEquivalentTo(tableInfo1);


                var expectedData1 = new[]
                {
                    new { Column1 = "Zeile 1, Spalte 1", Column2 = "Zeile 1, Spalte 2" },
                    new { Column1 = "Zeile 2, Spalte 1", Column2 = "Zeile 2, Spalte 2" }
                };

                tableInfo0.Data.ShouldBeEquivalentTo(expectedData1);



                // Tabelle mit Header, ohne automatische Erkennung
                TableGroup table2 = tables[1];
                TableInfoOptions options2 = new TableInfoOptions()
                {
                    DetermineHeaderRow = false,
                    HasHeaderRow = true,
                };
                TableInfo tableInfo2 = TableGroupUtility.GetTableInfo(table2, options2);

                var expectedInfo2 = new
                {
                    HasHeader = true,
                    Columns = new[]
    {
                        new { ColumnName = "Column 2" },
                        new { ColumnName = "Column 2" },
                    },
                };

                tableInfo2.Should().BeEquivalentTo(tableInfo2);


                var expectedData2 = new[]
                {
                    new { Column1 = "Zeile 1, Spalte 1", Column2 = "Zeile 1, Spalte 2" },
                    new { Column1 = "Zeile 2, Spalte 1", Column2 = "Zeile 2, Spalte 2" }
                };

                tableInfo0.Data.ShouldBeEquivalentTo(expectedData2);
            }
        }
    }
}
