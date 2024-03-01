using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Sereno.Office.Tables;
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

                AngularDocumentation angularDocumentation = new();
                angularDocumentation.Create();
                angularDocumentation.OpenDocumentation();


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

