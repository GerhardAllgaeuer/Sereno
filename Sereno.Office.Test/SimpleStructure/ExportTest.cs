using DocumentFormat.OpenXml.Packaging;
using FluentAssertions;
using Sereno.Office.Word;
using Sereno.Office.Word.SimpleStructure;
using Sereno.Office.Word.Word.SimpleStructure.Export;
using Sereno.Test;
using Sereno.Utilities;
using System.Data;

namespace Sereno.Office.Test.SimpleStructure
{
    [TestClass]
    public sealed class ExportTest
    {

        [TestMethod]
        public void Export_Html()
        {
            string filePath = $@"{CodeUtility.GetProjectRoot()}\Sereno.Office.Test\Templates\Test_0001.docx";

            using (WordprocessingDocument document = WordUtility.OpenWordDocument(filePath))
            {
                List<DocumentGroup> groups = [.. DocumentGroupUtility.GetDocumentGroups(document)];

                ExportOptions options = new()
                {
                    ExportDirectory = new DirectoryInfo(@"D:\Data\Sereno.Office\Document0001"),
                    Groups = groups,
                };

                HtmlExport htmlExport = new();
                htmlExport.Export(options);
            }
        }
    }
}
