using DocumentFormat.OpenXml.Packaging;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sereno.Office.Word;
using Sereno.Office.Word.SimpleStructure;
using Sereno.Test.Database;
using Sereno.Utilities;

namespace Sereno.Office.Test.SimpleStructure
{
    [TestClass]
    public sealed class TableReaderTests
    {

        [TestMethod]
        public void Read_Table_With_Unspecified_Width()
        {
            string filePath = $@"{CodeUtility.GetProjectRoot()}\Sereno.Office.Test\Documents\Tables.docx";

            using WordprocessingDocument document = WordUtility.OpenWordDocument(filePath);
            TableGroup tableGroup = DocumentGroupUtility.GetDocumentGroups(document)
                                        .OfType<TableGroup>()
                                        .ElementAt(2);


            tableGroup.TableInfo.Columns[0].ColumnWidth.Should().NotBe(null);
            tableGroup.TableInfo.Columns[0].ColumnWidth.Should().BeGreaterThan(0);
        }

        [TestMethod]
        public void Read_Table_Without_Header()
        {
            string filePath = $@"{CodeUtility.GetProjectRoot()}\Sereno.Office.Test\Documents\Tables.docx";

            using WordprocessingDocument document = WordUtility.OpenWordDocument(filePath);
            TableGroup tableGroup = DocumentGroupUtility.GetDocumentGroups(document)
                                          .OfType<TableGroup>()
                                          .First();

            var options = new TableInfoOptions { DetermineHeaderRow = true };
            var tableInfo = TableGroupUtility.GetTableInfo(tableGroup, options);

            tableInfo.Should().BeEquivalentTo(new
            {
                HasHeader = false,
                Columns = new[]
                {
                        new { ColumnName = "Column1" },
                        new { ColumnName = "Column2" },
                    },
            });

            var expectedData = new[]
            {
                new { Column1 = "Zeile 1, Spalte 1", Column2 = "Zeile 1, Spalte 2" },
                new { Column1 = "Zeile 2, Spalte 1", Column2 = "Zeile 2, Spalte 2" }
            };

            tableInfo.Data.ShouldBeEquivalentTo(expectedData);
        }

        [TestMethod]
        public void Read_Table_With_Header_Auto_Detect()
        {

        }


        [TestMethod]
        public void Read_Table_With_Header_Manual_Setting()
        {
            string filePath = $@"{CodeUtility.GetProjectRoot()}\Sereno.Office.Test\Documents\Tables.docx";

            using WordprocessingDocument document = WordUtility.OpenWordDocument(filePath);
            TableGroup tableGroup = DocumentGroupUtility.GetDocumentGroups(document)
                                        .OfType<TableGroup>()
                                        .ElementAt(1);

            var options = new TableInfoOptions
            {
                DetermineHeaderRow = false,
                HasHeaderRow = true
            };
            var tableInfo = TableGroupUtility.GetTableInfo(tableGroup, options);

            tableInfo.Should().BeEquivalentTo(new
            {
                HasHeader = true,
                Columns = new[]
                {
                        new { ColumnName = "Column1" },
                        new { ColumnName = "Column2" },
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
