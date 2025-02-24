using DocumentFormat.OpenXml.Packaging;
using Sereno.Office.Word;
using Sereno.Office.Word.SimpleStructure;

namespace Sereno.Documentation.FileAccess
{
    public class DocumentationFileReader
    {

        public static DocumentationFile Read(string filePath)
        {
            DocumentationFile result = new DocumentationFile()
            { 
                File = new FileInfo(filePath),
            };

            using WordprocessingDocument document = WordUtility.OpenWordDocument(filePath);

            List<DocumentGroup> paragraphs = [.. DocumentGroupUtility.GetDocumentGroups(document)];

            return result;
        }
    }
}
