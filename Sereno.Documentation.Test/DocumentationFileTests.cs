using ClosedXML.Excel;
using DocumentFormat.OpenXml.Packaging;
using FluentAssertions;
using Sereno.Documentation.FileAccess;
using Sereno.Office.Word.SimpleStructure;
using Sereno.Office.Word.Word.SimpleStructure.Export;
using Sereno.Office.Word;
using Sereno.Test.Excel;
using Sereno.Utilities;
using Sereno.Utilities.DirectorySync;
using Microsoft.Extensions.Configuration;
using Sereno.Documentation.Synchronization;

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

            List<DocumentationFile> files = DocumentationLibrary.ReadLibrary(rootDirectory);

            files.Count.Should().Be(3);

            DocumentationLibrary.WriteToExcel(files, templatePath, exportPath);

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

            List<DocumentationFile> files = DocumentationLibrary.ReadLibrary(rootDirectory);

            DocumentationLibrary.WriteToExcel(files, templatePath, exportPath);
        }



        [TestMethod]
        [TestProperty("Dev", "")]
        public void Export_Html_Production_Structure()
        {
            var configuration = ConfigurationUtility.GetConfiguration();
            string connectionString = configuration.GetConnectionString("Development_ConnectionString")!;

            string rootDirectory = $@"D:\Data\Dokumentation";
            string exportPath = $@"{CodeUtility.GetDataDirectory()}\Sereno.Office\Documents.xlsx";


            List<DocumentationFile> files = DocumentationLibrary.ReadLibrary(rootDirectory);

            foreach (DocumentationFile file in files)
            {
                using (WordprocessingDocument document = WordUtility.OpenWordDocument(file.Path))
                {
                    List<DocumentGroup> groups = [.. DocumentGroupUtility.GetDocumentGroups(document)];

                    ExportOptions options = new()
                    {
                        ExportDirectory = new DirectoryInfo(@$"D:\Data\Sereno.Office\Production\{file.RelativeDirectory}\{file.DocumentKey}"),
                    };

                    DocumentationExportOptions exportOptions = new DocumentationExportOptions()
                    {
                        RootDirectory = new DirectoryInfo(rootDirectory),
                    };
                    DocumentationExport.ExportHtml(file, exportOptions);
                }
            }
        }


        [TestMethod]
        [TestProperty("Dev", "")]
        public void Copy_Sereno_Code_To_Connexia()
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


            sourceDirectory = $@"D:\Projekte\Privat\Sereno\Sereno.Test.Extensions";
            targetDirectory = $@"D:\Projekte\Connexia\Connexia.root\Sereno.Test.Extensions";

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
