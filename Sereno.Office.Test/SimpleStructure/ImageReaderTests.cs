using DocumentFormat.OpenXml.Packaging;
using FluentAssertions;
using Sereno.Office.Word;
using Sereno.Office.Word.SimpleStructure;
using Sereno.Office.Word.Word.SimpleStructure;
using Sereno.Test.Database;
using Sereno.Utilities;

namespace Sereno.Office.Test.SimpleStructure
{
    [TestClass]
    public sealed class ImageReaderTests
    {

        [TestMethod]
        public void Read_Image()
        {
            string filePath = $@"{CodeUtility.GetProjectRoot()}\Sereno.Office.Test\Documents\Images.docx";

            using WordprocessingDocument document = WordUtility.OpenWordDocument(filePath);
            ImageGroup tableGroup = DocumentGroupUtility.GetDocumentGroups(document)
                                        .OfType<ImageGroup>()
                                        .ElementAt(0);

        }

    }
}
