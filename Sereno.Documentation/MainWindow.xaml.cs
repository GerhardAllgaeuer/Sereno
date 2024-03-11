using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Sereno.Office.Tables;
using Sereno.Office.Windows.Word;
using Sereno.Office.Word;
using System.Diagnostics;
using System.IO;
using System.Windows;
using Style = DocumentFormat.OpenXml.Wordprocessing.Style;

namespace Sereno.Documentation
{

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Main();
        }

        void Main()
        {

            try
            {

                SourceDocumentation introduction = new()
                {
                    TemplateFileName = $@"01 Einleitung.Template.docx",
                    ProjectName = $@"",
                };

                introduction.CreateDocumentation();




                SourceDocumentation angular = new()
                {
                    TemplateFileName = $@"11 Technik-Angular-Source.Template.docx",
                    ProjectName = $@"Sereno.Client",
                };

                angular.CreateSourceDocumentation();




                SourceDocumentation identityApi = new()
                {
                    TemplateFileName = $@"12 Technik-Identity-API.Source.Template.docx",
                    ProjectName = $@"Sereno.Identity.Api",
                };

                identityApi.CreateSourceDocumentation();




                SourceDocumentation identityDto = new()
                {
                    TemplateFileName = $@"13 Technik-Identity-DTO.Source.Template.docx",
                    ProjectName = $@"Sereno.Identity.Dto",
                };

                identityDto.CreateSourceDocumentation();




                textBlockMessage.Text = "";

            }
            catch (System.Exception ex)
            {
                textBlockMessage.Text = $"{ex.Message}";
                Console.WriteLine($"Ein Fehler ist aufgetreten: {ex.Message}");
            }

        }



    }
}

