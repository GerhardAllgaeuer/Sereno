using DocumentFormat.OpenXml.Packaging;
using Sereno.Documentation.FileAccess;
using Sereno.Office.Word;
using Sereno.Office.Word.SimpleStructure;
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
    }
}
