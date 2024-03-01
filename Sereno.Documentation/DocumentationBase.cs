using DocumentFormat.OpenXml.Packaging;
using Sereno.Office.Tables;
using Sereno.Office.Word;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.Marshalling.IIUnknownCacheStrategy;

namespace Sereno.Documentation
{
    public class DocumentationBase
    {

        public string? SourceRoot { get; set; } = @"D:\Projekte\Privat\Sereno";

        public string? DocumentationRoot { get; set; } = @"D:\daten\age\Nextcloud\Wissen\Angular\Documentation";

        public string? TemplateFileName { get; set; }

        public string? TemplateFilePath => $@"{this.DocumentationRoot}\{this.TemplateFileName}";

        public string? DestinationFileName { get; set; }

        public string? DestinationFilePath => $@"{this.DocumentationRoot}\{this.DestinationFileName}";

        public string? ProjectName { get; set; }

        public string? ProjectDirectory => $@"{this.SourceRoot}\{this.ProjectName}";


        public virtual void Create()
        {
            File.Copy(this.TemplateFilePath!, this.DestinationFilePath!, true);

        }



        public void OpenDocumentation()
        {
            WordUtility.OpenDocument(this.DestinationFilePath!);
        }

        protected virtual void CreateProjectStructureTable(WordprocessingDocument document)
        {
            System.Data.DataTable? table = this.GetProjectFolders(this.ProjectDirectory!);

            if (table != null)
            {
                TableOption options = new TableOption()
                {
                    StartRow = 1,
                    HasHeader = true,
                };
                WordUtility.FillTable(document, table, "SourceStructureDescriptionTable", options);
            }
        }


        /// <summary>
        /// Auslesen der Projektordner
        /// </summary>
        protected virtual System.Data.DataTable? GetProjectFolders(string path)
        {
            DirectoryInfo dirInfo = new(path);
            SourceInfo? sourceInfo = SourceInfoUtility.GetSourceInfo(dirInfo.FullName);
            List<SourceInfo> sourceInfos = SourceInfoUtility.Flatten(sourceInfo!);

            Office.Tables.TableInfo tableInfo = new()
            {
                Columns =
                [
                    new TableColumn() { ColumnName = "Ordner", SourceProperty = nameof(SourceInfo.Location) },
                    new TableColumn() { ColumnName = "Beschreibung", SourceProperty = nameof(SourceInfo.Description) },
                ]
            };

            System.Data.DataTable? table = TableUtility.GetDataSetFromObjectList(sourceInfos, tableInfo);

            return table;
        }



    }
}
