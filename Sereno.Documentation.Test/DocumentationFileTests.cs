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

            DocumentationFile documentation = DocumentationFileReader.Read(filePath);

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
    }
}
