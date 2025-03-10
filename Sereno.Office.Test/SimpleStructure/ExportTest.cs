using DocumentFormat.OpenXml.Packaging;
using Sereno.Office.Word;
using Sereno.Office.Word.SimpleStructure;
using Sereno.Office.Word.Word.SimpleStructure.Export;
using Sereno.Utilities;

namespace Sereno.Office.Test.SimpleStructure
{
    [TestClass]
    public sealed class ExportTest
    {

        [TestMethod]
        public void Export_Html_With_All_Types()
        {
            string filePath = $@"{CodeUtility.GetProjectRoot()}\Sereno.Office.Test\Documents\All_Types.docx";

            using WordprocessingDocument document = WordUtility.OpenWordDocument(filePath);
            List<DocumentGroup> groups = [.. DocumentGroupUtility.GetDocumentGroups(document)];

            ExportOptions options = new()
            {
                ExportDirectory = new DirectoryInfo(@"D:\Data\Sereno.Office\All_Types"),
            };

            HtmlExport htmlExport = new();
            htmlExport.Export(groups, options);
        }


        [TestMethod]
        public void Export_Html_To_Show_Styles()
        {
            string filePath = $@"{CodeUtility.GetProjectRoot()}\Sereno.Office.Test\Documents\Long_Document.docx";

            using WordprocessingDocument document = WordUtility.OpenWordDocument(filePath);
            List<DocumentGroup> groups = [.. DocumentGroupUtility.GetDocumentGroups(document)];

            ExportOptions options = new()
            {
                ExportDirectory = new DirectoryInfo(@"D:\Data\Sereno.Office\Long_Document"),
            };

            HtmlExport htmlExport = new();
            htmlExport.Export(groups, options);
        }
    }
}
