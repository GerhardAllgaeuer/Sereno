using DocumentFormat.OpenXml.Packaging;
using FluentAssertions;
using Sereno.Office.Word;
using Sereno.Office.Word.SimpleStructure;
using Sereno.Office.Word.Word.SimpleStructure.Converter;
using Sereno.Utilities;

namespace Sereno.Office.Test.SimpleStructure
{
    [TestClass]
    public sealed class ExportTest
    {

        [TestMethod]
        public void Export_Html_Files_And_Check_Relative_Path()
        {
            string filePath = $@"{CodeUtility.GetProjectRoot()}\Sereno.Office.Test\Documents\All_Types.docx";

            using WordprocessingDocument document = WordUtility.OpenWordDocument(filePath);
            List<DocumentGroup> groups = [.. DocumentGroupUtility.GetDocumentGroups(document)];

            DirectoryInfo exportDirectory = new DirectoryInfo(@"D:\Data\Sereno.ImageExport");

            // Mit Default Options, Files direkt ins Root
            HtmlConverterOptions options = new HtmlConverterOptions()
            {
                ExportRootDirectory = exportDirectory,
            };
            HtmlConverter htmlExport = new(options);
            htmlExport.Convert(groups);
            htmlExport.SaveFiles();

            File.Exists($@"{exportDirectory.FullName}\Image0001.png").Should().BeTrue("Image0001.png nicht vorhanden");
            htmlExport.HtmlDocument.Should().Contain("<img src=\"Image0001.png\"");


            // Mit Unterordner Images
            exportDirectory = new DirectoryInfo(@"D:\Data\Sereno.ImageExport\images");
            options = new HtmlConverterOptions()
            {
                RelativeImageDirectory = "images",
                RelativeImageHtmlDirectory = "images",
                ExportRootDirectory = exportDirectory,
            };
            htmlExport = new(options);
            htmlExport.Convert(groups);
            htmlExport.SaveFiles();

            File.Exists($@"{exportDirectory.FullName}\{options.RelativeImageDirectory}\Image0001.png").Should().BeTrue("Image0001.png nicht vorhanden");
            htmlExport.HtmlDocument.Should().Contain("<img src=\"images/Image0001.png\"");


        }



        [TestMethod]
        public void Export_Html_With_All_Types()
        {
            string filePath = $@"{CodeUtility.GetProjectRoot()}\Sereno.Office.Test\Documents\All_Types.docx";

            using WordprocessingDocument document = WordUtility.OpenWordDocument(filePath);
            List<DocumentGroup> groups = [.. DocumentGroupUtility.GetDocumentGroups(document)];

            DirectoryInfo exportDirectory = new DirectoryInfo(@"D:\Data\Sereno.Office\All_Types");

            HtmlConverterOptions options = new HtmlConverterOptions()
            {
                RelativeImageDirectory = "images",
                RelativeImageHtmlDirectory = "images",
                ExportRootDirectory = exportDirectory,
            };

            HtmlConverter htmlExport = new(options);
            htmlExport.Convert(groups);
            htmlExport.SaveDocument();
            htmlExport.SaveStyleSheet();
            htmlExport.SaveFiles();
        }


        [TestMethod]
        public void Export_Html_To_Show_Styles()
        {
            string filePath = $@"{CodeUtility.GetProjectRoot()}\Sereno.Office.Test\Documents\Long_Document.docx";

            using WordprocessingDocument document = WordUtility.OpenWordDocument(filePath);
            List<DocumentGroup> groups = [.. DocumentGroupUtility.GetDocumentGroups(document)];

            DirectoryInfo exportDirectory = new DirectoryInfo(@"D:\Data\Sereno.Office\Long_Document");

            HtmlConverterOptions options = new HtmlConverterOptions()
            {
                RelativeImageDirectory = "images",
                RelativeImageHtmlDirectory = "images",
                ExportRootDirectory = exportDirectory,
            };

            HtmlConverter htmlExport = new(options);
            htmlExport.Convert(groups);
            htmlExport.SaveDocument();
            htmlExport.SaveStyleSheet();
            htmlExport.SaveFiles();
        }
    }
}
