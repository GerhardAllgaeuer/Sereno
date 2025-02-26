using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Sereno.Utilities.TableConverter;
using Sereno.Office.Word;
using System.IO;

namespace Sereno.Documentation
{
    public class DocumentationBase
    {

        public string? SourceRoot { get; set; } = @"D:\Projekte\Privat\Sereno";

        public string? DocumentationRoot { get; set; } = @"D:\daten\age\Nextcloud\Wissen\Angular\Documentation";

        public string? TemplateFileName { get; set; }

        public string? TemplateFilePath => $@"{this.DocumentationRoot}\Templates\{this.TemplateFileName}";

        public string? DestinationFileName { get; set; }

        public string? DestinationFilePath => $@"{this.DocumentationRoot}\{this.DestinationFileName}";

        public string? ProjectName { get; set; }

        public string? ProjectDirectory => $@"{this.SourceRoot}\{this.ProjectName}";


        protected virtual void Create()
        {

            this.DestinationFileName = this.TemplateFileName!.Replace(".Template", "");

            File.Copy(this.TemplateFilePath!, this.DestinationFilePath!, true);

        }



        public void OpenDocumentation()
        {
            WordUtility.OpenDocument(this.DestinationFilePath!);
        }

        protected virtual void CreateProjectStructureTable(WordprocessingDocument document)
        {
            SourceInfoOptions options = new()
            {
                Root = this.ProjectDirectory,
                Types = new Dictionary<SourceInfoTypes, string?>
                {
                    { SourceInfoTypes.Directory, null },
                }
            };

            SourceInfo? rootInfo = SourceInfoUtility.GetSourceInfo(options);
            List<SourceInfo> sourceInfos = SourceInfoUtility.Flatten(rootInfo!);
            System.Data.DataTable? table = this.GetProjectFolders(sourceInfos);

            if (table != null)
            {
                TableOption tableOptions = new TableOption()
                {
                    StartRow = 1,
                    HasHeader = true,
                };
                WordUtility.FillTable(document, table, "SourceStructureDescriptionTable", tableOptions);
            }
        }


        public void WriteSourceCode(WordprocessingDocument document)
        {
            SourceInfoOptions options = new()
            {
                Root = this.ProjectDirectory,
                Types = new Dictionary<SourceInfoTypes, string?>
                {
                    { SourceInfoTypes.File, null },
                }
            };

            SourceInfo? rootInfo = SourceInfoUtility.GetSourceInfo(options);
            List<SourceInfo> sourceInfos = SourceInfoUtility.Flatten(rootInfo!);

            BookmarkStart? bookmarkStart = WordUtility.GetBookmark(document, "SourceCode");

            if (bookmarkStart != null)
            {
                var last = bookmarkStart.Parent!;

                foreach (SourceInfo sourceInfo in sourceInfos)
                {
                    if (sourceInfo.Type == SourceInfoTypes.File)
                    {
                        TextFormatOptions headerFormat = new()
                        {
                            Style = TextFormatStyles.Heading2,
                        };

                        last = WordUtility.AddParagraph(last!, sourceInfo.Location!, headerFormat);


                        TextFormatOptions codeFormat = new()
                        {
                            Style = TextFormatStyles.Code,
                        };

                        string[] lines = File.ReadAllLines(sourceInfo.AbsoluteLocation!);
                        int i = 0;

                        foreach (string line in lines)
                        {
                            if (last != null)
                            {
                                last = WordUtility.AddParagraph(last, line, codeFormat);

                                //if (i > 10)
                                    //break;
                            }

                            i++;
                        }
                    }
                }
            }
        }


        /// <summary>
        /// Auslesen der Projektordner
        /// </summary>
        protected virtual System.Data.DataTable? GetProjectFolders(List<SourceInfo> sourceInfos)
        {

            MappingInfo tableInfo = new()
            {
                Columns =
                [
                    new MappingColumn() { ColumnName = "Ordner", SourceProperty = nameof(SourceInfo.Location) },
                    new MappingColumn() { ColumnName = "Beschreibung", SourceProperty = nameof(SourceInfo.Description) },
                ]
            };

            System.Data.DataTable? table = TableConverterUtility.DataTableFromObjectList(sourceInfos, tableInfo);

            return table;
        }



    }
}
