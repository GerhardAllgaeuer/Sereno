using FluentAssertions;
using Sereno.Documentation.FileAccess;
using Sereno.Test;
using Sereno.Utilities.DirectorySync;

namespace Sereno.Documentation
{
    [TestClass]
    public sealed class DocumentationFileTests
    {

        [TestMethod]
        public void Read_Word_File()
        {
            string filePath = $@"{TestUtility.GetProjectRoot()}\Sereno.Documentation.Test\Documents\Documentation_0001.docx";

            DocumentationFile? documentation = DocumentationFileReader.Read(filePath);

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
        public void Read_Structure()
        {
            string rootDirectory = $@"{TestUtility.GetProjectRoot()}\Sereno.Documentation.Test\DocumentsLibrary";
            string exportPath = $@"{TestUtility.GetDataDirectory()}\Sereno.Office\TestDocuments.xlsx";
            string templatePath = $@"{TestUtility.GetProjectRoot()}\\Sereno.Documentation.Dal\FileAccess\DocumentsTemplate.xlsx";

            List<DocumentationFile> files = DocumentationLibraryUtility.ReadLibrary(rootDirectory);

            files.Count.Should().Be(3);

            DocumentationLibraryUtility.WriteToExcel(files, templatePath, exportPath);
        }


        [TestMethod]
        [TestProperty("Dev", "")]
        public void Read_Production_Structure()
        {
            string rootDirectory = $@"\\conad01\info\EDV\Dokumentation";
            string exportPath = $@"{TestUtility.GetDataDirectory()}\Sereno.Office\Documents.xlsx";
            string templatePath = $@"{TestUtility.GetProjectRoot()}\\Sereno.Documentation.Dal\FileAccess\DocumentsTemplate.xlsx";

            List<DocumentationFile> files = DocumentationLibraryUtility.ReadLibrary(rootDirectory);

            DocumentationLibraryUtility.WriteToExcel(files, templatePath, exportPath);
        }

        [TestMethod]
        [TestProperty("Dev", "")]
        public void Sync_Sereno_Code()
        {
            string sourceDirectory = $@"D:\Projekte\Privat\Sereno\Sereno.Office.Excel";
            string targetDirectory = $@"D:\Projekte\Connexia\Connexia.root\Sereno.Office.Excel";

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
