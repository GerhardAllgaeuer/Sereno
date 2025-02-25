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

        }

        [TestMethod]
        public void ReadStuctrue()
        {
            string rootDirectory = $@"{TestUtility.GetProjectRoot()}\Sereno.Documentation.Test\DocumentsLibrary";

            List<DocumentationFile> files = DocumentationLibraryUtility.ReadLibrary(rootDirectory);
        }
    }
}
