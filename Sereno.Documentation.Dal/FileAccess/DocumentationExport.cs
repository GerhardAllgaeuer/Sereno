using Sereno.Office.Word.Word.SimpleStructure.Export;

namespace Sereno.Documentation.FileAccess
{
    public class DocumentationExport
    {
        public static void ExportHtml(DocumentationFile file, DocumentationExportOptions options)
        {
            ExportOptions exportOptions = new ExportOptions()
            {
                ExportDirectory = new DirectoryInfo($@"{options.RootDirectory}\{file.RelativeDirectory}"),
            };

            HtmlExport htmlExport = new();
            htmlExport.Export(file.Contents, exportOptions);
        }
    }
}
