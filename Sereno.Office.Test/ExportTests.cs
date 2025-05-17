using DocumentFormat.OpenXml.Packaging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sereno.Office.Word;
using Sereno.Test;
using Sereno.Utilities;

namespace Sereno.Office.Test
{
    [TestClass]
    public sealed class ExportTests
    {
        [TestMethod]
        public void Export_Word_Document_To_Csv()
        {
            string wordFilePath = $@"{CodeUtility.GetProjectRoot()}\Sereno.Office.Test\Documents\List_Paragraphs.docx";
            string exportFilePath = $@"{CodeUtility.GetDataDirectory()}\Sereno.Office\Test_0001.csv";

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
