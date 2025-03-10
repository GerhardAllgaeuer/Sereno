﻿using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;

namespace Sereno.Documentation
{
    public class SourceDocumentation : DocumentationBase
    {


        public SourceDocumentation()
        {
            this.TemplateFileName = $@"11 Technik-Angular-Source.Template.docx";
            this.DestinationFileName = $@"11 Technik-Angular-Source.docx";
            this.ProjectName = $@"Sereno.Client";
        }



        // Definieren Sie hier die Methode, die als Action verwendet wird
        void HandleSerenoParagraph(OpenXmlElement element, string serenoText)
        {
            if (element == null)
            {
                return;
            }

            // Implementieren Sie Ihre Logik hier. 'element' ist das OpenXmlElement nach dem Sereno-Absatz, 'text' ist der Text des Sereno-Absatzes.
            Console.WriteLine($"Nächster Nicht-Sereno-Absatz: {element.InnerText}");
            Console.WriteLine($"Text des gelöschten Sereno-Absatzes: {serenoText}");
        }

        public void CreateDocumentation()
        {
            base.Create();

            using WordprocessingDocument document = WordprocessingDocument.Open(this.DestinationFilePath!, true);
            try
            {
                document.Save();
            }
            catch
            {
                throw;
            }
            finally
            {
                if (document != null)
                {
                    document.Dispose();
                }
            }
        }



        public void CreateSourceDocumentation()
        {
            base.Create();

            using WordprocessingDocument document = WordprocessingDocument.Open(this.DestinationFilePath!, true);
            try
            {
                //SerenoWordUtility.ProcessDocument(document, HandleSerenoParagraph);

                this.CreateProjectStructureTable(document);
                this.WriteSourceCode(document);


                document.Save();
            }
            catch
            {
                throw;
            }
            finally
            {
                if (document != null)
                {
                    document.Dispose();
                }
            }
        }
    }
}
