using DocumentFormat.OpenXml.Packaging;
using Sereno.Documentation.Synchronization;
using Sereno.Office.Excel.Excel.Writer;
using Sereno.Office.Excel.Writer;
using Sereno.Office.Word.SimpleStructure;
using Sereno.Office.Word;
using Sereno.Utilities.TableConverter;
using System.Data;
using System.Diagnostics;
using Sereno.Office.Word.Word.SimpleStructure.Export;
using System;
using Sereno.Documentation.DataAccess;
using Sereno.Documentation.DataAccess.Entities;

namespace Sereno.Documentation.FileAccess
{
    public class DocumentationLibraryUtility
    {

        public static List<DocumentationFile> ReadLibrary(string directory)
        {
            List<DocumentationFile> result = [];

            if (!Directory.Exists(directory))
            {
                Console.WriteLine($"Directory does not exist: {directory}");
                return result;
            }

            ProcessDirectory(directory, directory, result);
            return result;
        }

        public static void SyncLibrary(SyncOptions options)
        {
            using var context = AppDbContextFactory.CreateDbContext(options.DatabaseConnectionString);
            List<DocumentationFile> files = ReadLibrary(options.DocumentsDirectory.FullName);

            foreach (DocumentationFile file in files)
            {
                using (WordprocessingDocument document = WordUtility.OpenWordDocument(file.Path))
                {
                    List<DocumentGroup> groups = [.. DocumentGroupUtility.GetDocumentGroups(document)];

                    DocumentationExportOptions exportOptions = new DocumentationExportOptions()
                    {
                        RootDirectory = options.HtmlExportDirectory,
                    };
                    DocumentationExport.ExportHtml(file, exportOptions);
                }

                SaveFileToDatabase(context, file);
            }
        }

        private static void SaveFileToDatabase(AppDbContext dbContext, DocumentationFile file)
        {
            var document = new Document
            {
                Id = dbContext.GetPrimaryKey(),
                Title = file.Title,
                Content = "",
            };

            dbContext.Documents.Add(document);
            dbContext.SaveChanges();
        }


        public static void WriteToExcel(List<DocumentationFile> files, string template, string path)
        {
            MappingInfo tableInfo = new MappingInfo()
            {
                Columns =
                [
                    new MappingColumn() { ColumnName = nameof(DocumentationFile.Title), SourceProperty = nameof(DocumentationFile.Title) },
                    new MappingColumn() { ColumnName = nameof(DocumentationFile.Author), SourceProperty = nameof(DocumentationFile.Author) },
                    new MappingColumn() { ColumnName = nameof(DocumentationFile.RelativePath), SourceProperty = nameof(DocumentationFile.RelativePath) },
                    new MappingColumn() { ColumnName = nameof(DocumentationFile.Path), SourceProperty = nameof(DocumentationFile.Path) },
                ]
            };

            DataTable? table = TableConverterUtility.DataTableFromObjectList(files, tableInfo);


            File.Delete(path);
            File.Copy(template, path);
            DataSet dataSet = TableConverterUtility.DataSetFromObjectList<DocumentationFile>(files, tableInfo);
            DataSetInsertOptions options = new DataSetInsertOptions()
            {
                StartRow = 4,
                TableColor = TableColors.Blue,
            };
            ExcelWriterUtility.InsertDataSetInExcel(path, dataSet, options);
        }



        private static bool IncludeFile(string file)
        {
            bool result = true;

            if (!file.EndsWith(".docx"))
                result = false;

            return result;
        }


        private static void ProcessDirectory(string rootdirectory, string directory, List<DocumentationFile> processedFiles)
        {
            bool limitFiles = true;
            int currentFile = 0;
            int countFiles = 2;
            int currentDirectory = 0;
            int countDirectories = 1;

            foreach (string file in Directory.GetFiles(directory))
            {
                if (IncludeFile(file))
                {
                    try
                    {
                        DocumentationFile? docFile = DocumentationFileReader.Read(new DocumentationReaderOptions() { FilePath = file });
                        if (docFile != null)
                        {
                            FileInfo fileInfo = new FileInfo(file);

                            docFile.RelativePath = fileInfo.FullName.Replace(rootdirectory, "");
                            if (docFile.RelativePath.StartsWith('\\'))
                            {
                                docFile.RelativePath = docFile.RelativePath.Substring(1);
                            }

                            docFile.RelativeDirectory = fileInfo!.Directory!.FullName.Replace(rootdirectory, "");
                            if (docFile.RelativeDirectory.StartsWith('\\'))
                            {
                                docFile.RelativeDirectory = docFile.RelativeDirectory.Substring(1);
                            }

                            processedFiles.Add(docFile);

                            Debug.WriteLine(docFile.RelativePath);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error processing file {file}: {ex.Message}");
                    }

                    currentFile++;
                }

                if (limitFiles &&
                    currentFile > countFiles)
                    break;
            }

            foreach (string subDirectory in Directory.GetDirectories(directory))
            {
                ProcessDirectory(rootdirectory, subDirectory, processedFiles);

                currentDirectory++;

                if (limitFiles &
                    currentDirectory > countDirectories)
                    break;
            }
        }
    }
}
