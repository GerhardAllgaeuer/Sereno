using DocumentFormat.OpenXml.Packaging;
using Sereno.Office.Word;
using Sereno.Office.Word.SimpleStructure;
using DocumentFormat.OpenXml.Wordprocessing;
using System.Data;

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

            List<DocumentGroup> groups = [.. DocumentGroupUtility.GetDocumentGroups(document)];

            ParagraphGroup? title = GetTitle(groups);
            TableGroup? documentDataTable = GetDocumentDataTable(groups);
            if (documentDataTable != null)
            {
                TableInfo tableInfo = TableGroupUtility.GetTableInfo(documentDataTable);
            }

            return result;
        }


        private static TableGroup? GetDocumentDataTable(List<DocumentGroup> groups)
        {
            for (int i = 0; i < 5; i++)
            {
                if (groups[i] is TableGroup group)
                {
                    return group;
                }
            }

            return null;
        }

        private static ParagraphGroup? GetTitle(List<DocumentGroup> groups)
        {
            for (int i = 0; i < 3; i++)
            {
                if (groups[i] is ParagraphGroup pargraphGroup)
                {
                    if (pargraphGroup.StyleNameEn != null &&
                        pargraphGroup.StyleNameEn == "Title")
                    {
                        return pargraphGroup;
                    }
                }
            }

            return null;
        }

    }
}
