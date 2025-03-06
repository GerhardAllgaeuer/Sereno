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
        public void Read_Images()
        {
            string filePath = $@"{CodeUtility.GetProjectRoot()}\Sereno.Office.Test\Documents\Images.docx";

            using WordprocessingDocument document = WordUtility.OpenWordDocument(filePath);


            List<ImageGroup> groups = DocumentGroupUtility.GetDocumentGroups(document)
                                        .OfType<ImageGroup>()
                                        .ToList();

            ImageGroup imageGroup0 = groups.ElementAt(0);

            imageGroup0.Images.Count.Should().Be(1);


            ImageGroup imageGroup1 = groups.ElementAt(1);

            imageGroup1.Images.Count.Should().Be(2);

        }

    }
}
