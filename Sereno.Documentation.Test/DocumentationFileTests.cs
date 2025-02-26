using FluentAssertions;
using Sereno.Documentation.FileAccess;
using Sereno.Test;

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

            List<DocumentationFile> files = DocumentationLibraryUtility.ReadLibrary(rootDirectory);

            files.Count.Should().Be(3);
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
    }
}
