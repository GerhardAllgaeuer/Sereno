using DocumentFormat.OpenXml.Packaging;
using FluentAssertions;
using Sereno.Office.Word;
using Sereno.Office.Word.SimpleStructure;

namespace Sereno.Office.Test
{
    [TestClass]
    public sealed class ExportTests
    {
        [TestMethod]
        public void Export_Document()
        {
            string wordFilePath = @"D:\Projekte\Privat\Sereno\Sereno.Office.Test\Templates\Test_0001.docx";
            string exportFilePath = @"D:\Data\Sereno.Office\Test_0001.csv";

            using (WordprocessingDocument? document = WordUtility.OpenWordDocument(wordFilePath))
            {
                if (document != null)
                {
                    WordExportUtility.ExportDocumentStructureToCsv(document, exportFilePath);
                }
            }
        }
    }
}
