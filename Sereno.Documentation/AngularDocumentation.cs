using DocumentFormat.OpenXml.Packaging;
using Sereno.Office.Tables;
using Sereno.Office.Word;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sereno.Documentation
{
    public class AngularDocumentation : DocumentationBase
    {


        public AngularDocumentation()
        {
            this.TemplateFileName = $@"Angular-Template.docx";
            this.DestinationFileName = $@"Angular-Source.docx";
            this.ProjectName = $@"Sereno.Client";
        }


        public override void Create()
        {
            base.Create();

            using (WordprocessingDocument document = WordprocessingDocument.Open(this.DestinationFilePath!, true))
            {
                this.CreateProjectStructureTable(document);

                document.Save();


            }
        }
    }
}
