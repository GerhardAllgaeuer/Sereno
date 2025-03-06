using ClosedXML.Excel;
using DocumentFormat.OpenXml.Packaging;
using FluentAssertions;
using Microsoft.CodeCoverage.Core.Reports.Coverage;
using Sereno.Documentation.FileAccess;
using Sereno.Office.Word.SimpleStructure;
using Sereno.Office.Word.Word.SimpleStructure.Export;
using Sereno.Office.Word;
using Sereno.Test;
using Sereno.Test.Excel;
using Sereno.Utilities;
using Sereno.Utilities.DirectorySync;

namespace Sereno.Documentation
{
    [TestClass]
    public sealed class DocumentationFileTests
    {

        [TestMethod]
        public void Read_Header_Information()
        {
            string filePath = $@"{CodeUtility.GetProjectRoot()}\Sereno.Documentation.Test\Documents\Documentation_0001.docx";

            DocumentationFile? documentation = DocumentationFileReader.Read(new DocumentationReaderOptions() { FilePath = filePath });

            var expectedInfo = new
            {
                Author = "mitarbeiter1@test.com",
                InfoReceivers = "alle",
                NextCheck = new DateTime(2024, 11, 1),
                Type = "Anleitung",
            };

            documentation.Should().BeEquivalentTo(expectedInfo);
        }



        [TestMethod]
        public void Read_And_Export_Library()
        {
            string rootDirectory = $@"{CodeUtility.GetProjectRoot()}\Sereno.Documentation.Test\DocumentsLibrary";
            string exportPath = $@"{CodeUtility.GetDataDirectory()}\Sereno.Office\TestDocuments.xlsx";
            string templatePath = $@"{CodeUtility.GetProjectRoot()}\\Sereno.Documentation.Dal\FileAccess\DocumentsTemplate.xlsx";

            List<DocumentationFile> files = DocumentationLibraryUtility.ReadLibrary(rootDirectory);

            files.Count.Should().Be(3);

            DocumentationLibraryUtility.WriteToExcel(files, templatePath, exportPath);

            using var workbook = new XLWorkbook(exportPath);
            workbook.Table().HaveRowCount(3);

            workbook.Table().Column("Author").ContainsValues(
                [
                    "mitarbeiter1@test.com",
                    "",
                    "mitarbeiter1@test.com",
                ]);
        }


        [TestMethod]
        [TestProperty("Dev", "")]
        public void Read_Production_Structure()
        {
            string rootDirectory = $@"\\conad01\info\EDV\Dokumentation";
            string exportPath = $@"{CodeUtility.GetDataDirectory()}\Sereno.Office\Documents.xlsx";
            string templatePath = $@"{CodeUtility.GetProjectRoot()}\\Sereno.Documentation.Dal\FileAccess\DocumentsTemplate.xlsx";

            List<DocumentationFile> files = DocumentationLibraryUtility.ReadLibrary(rootDirectory);

            DocumentationLibraryUtility.WriteToExcel(files, templatePath, exportPath);
        }

        [TestMethod]
        [TestProperty("Dev", "")]
        public void Export_Html_Production_Structure()
        {
            string rootDirectory = $@"\\conad01\info\EDV\Dokumentation";
            string exportPath = $@"{CodeUtility.GetDataDirectory()}\Sereno.Office\Documents.xlsx";
            string templatePath = $@"{CodeUtility.GetProjectRoot()}\\Sereno.Documentation.Dal\FileAccess\DocumentsTemplate.xlsx";

            List<DocumentationFile> files = DocumentationLibraryUtility.ReadLibrary(rootDirectory);

            foreach (DocumentationFile file in files)
            {
                using (WordprocessingDocument document = WordUtility.OpenWordDocument(file.Path))
                {
                    List<DocumentGroup> groups = [.. DocumentGroupUtility.GetDocumentGroups(document)];

                    ExportOptions options = new()
                    {
                        ExportDirectory = new DirectoryInfo(@$"D:\Data\Sereno.Office\Production\{file.RelativeDirectory}\{file.Key}"),
                        Groups = groups,
                    };

                    HtmlExport htmlExport = new();
                    htmlExport.Export(options);
                }
            }
        }


        [TestMethod]
        [TestProperty("Dev", "")]
        public void Sync_Sereno_Code()
        {
            string sourceDirectory = $@"D:\Projekte\Privat\Sereno\Sereno.Office.Excel";
            string targetDirectory = $@"D:\Projekte\Connexia\Connexia.root\Sereno.Office.Excel";

            CopySource(sourceDirectory, targetDirectory);


            sourceDirectory = $@"D:\Projekte\Privat\Sereno\Sereno.Office.Word";
            targetDirectory = $@"D:\Projekte\Connexia\Connexia.root\Sereno.Office.Word";

            CopySource(sourceDirectory, targetDirectory);



            sourceDirectory = $@"D:\Projekte\Privat\Sereno\Sereno.Utilities";
            targetDirectory = $@"D:\Projekte\Connexia\Connexia.root\Sereno.Utilities";

            CopySource(sourceDirectory, targetDirectory);



        }

        private void CopySource(string sourceDirectory, string targetDirectory)
        {
            DirectorySyncUtility.SyncDirectories(sourceDirectory, targetDirectory);
            DirectorySyncUtility.RemoveBinAndObjDirectory(targetDirectory);
            DirectorySyncUtility.RemoveGitFiles(targetDirectory);

        }
    }
}
