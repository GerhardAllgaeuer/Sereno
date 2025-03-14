using Sereno.Office.Excel.Excel.Writer;
using Sereno.Office.Excel.Writer;
using Sereno.Utilities.TableConverter;
using System.Data;
using System.Diagnostics;
using Sereno.Documentation.DataAccess;
using Sereno.Documentation.DataAccess.Entities;
using Sereno.Documentation.FileAccess;
using Sereno.Documentation.Synchronization;
using Sereno.Utilities;
using Sereno.Office.Word.Word.SimpleStructure.Converter;
using Microsoft.EntityFrameworkCore;

namespace Sereno.Documentation
{
    public class DocumentationLibrary
    {
        public required DirectoryInfo SourceRootDirectory { get; set; }

        public DirectoryInfo? TargetFilesDirectory { get; set; }

        public required string DatabaseConnectionString { get; set; }


        public void SyncLibrary(SyncOptions options)
        {
            List<DocumentationFile> files = ReadLibrary(this.SourceRootDirectory.FullName);

            foreach (DocumentationFile file in files)
            {
                if (file.DocumentKey.Contains("Beschaffung\\Organisation_Beschaffung_Anleitung"))
                {
                }

                SaveData(file);
            }
        }

        private string GetRelativeImageHtmlDirectory(DocumentationFile file)
        {
            string documentPath = file.DocumentKey.Replace(@"\", "/");
            return $@"src/assets/images/{documentPath}";
        }

        public string SaveData(DocumentationFile file)
        {
            using var context = AppDbContextFactory.CreateDbContext(this.DatabaseConnectionString);
            HtmlConverterOptions converterOptions = new HtmlConverterOptions()
            {
                RelativeImageHtmlDirectory = GetRelativeImageHtmlDirectory(file), // im Verzeichnis Images
                RelativeImageDirectory = file.DocumentKey,  // in den Unterordner mit dem Dokumentationspfad
                ExportRootDirectory = this.TargetFilesDirectory,
            };

            // Html Datei erstellen
            HtmlConverter converter = new(converterOptions);
            converter.Convert(file.Contents);


            // Dateien im Client Speichern
            converter.SaveFiles();

            // Daten in der DB Speichern
            SaveFileToDatabase(context, file, converter);

            return converter.Document;
        }


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


        public void CleanupTargetFilesDirectory()
        {
            DirectoryUtility.CleanUpDirectory(this.TargetFilesDirectory);
        }

        public void DeleteAllDocumentsInDatabase()
        {
            using var appDbContext = AppDbContextFactory.CreateDbContext(this.DatabaseConnectionString);
            appDbContext.Database.ExecuteSqlRaw("DELETE FROM docDocument");
        }


        private static void SaveFileToDatabase(AppDbContext dbContext, DocumentationFile file, HtmlConverter converter)
        {
            var document = new Document
            {
                Id = dbContext.GetPrimaryKey(),
                LibraryPath = file.RelativeSourceFilePath,
                DocumentKey = file.DocumentKey,
                Title = file.Title,
                Author = file.Author,
                NextCheck = file.NextCheck,
                Content = file.PlainText,
                HtmlContent = converter.Document,
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
                    new MappingColumn() { ColumnName = nameof(DocumentationFile.RelativeSourceFilePath), SourceProperty = nameof(DocumentationFile.RelativeSourceFilePath) },
                    new MappingColumn() { ColumnName = nameof(DocumentationFile.Path), SourceProperty = nameof(DocumentationFile.Path) },
                ]
            };

            DataTable? table = TableConverterUtility.DataTableFromObjectList(files, tableInfo);


            File.Delete(path);
            File.Copy(template, path);
            DataSet dataSet = TableConverterUtility.DataSetFromObjectList(files, tableInfo);
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
            bool limitFiles = false;
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

                            docFile.RelativeSourceFilePath = fileInfo.FullName.Replace(rootdirectory, "");
                            if (docFile.RelativeSourceFilePath.StartsWith('\\'))
                            {
                                docFile.RelativeSourceFilePath = docFile.RelativeSourceFilePath.Substring(1);
                            }

                            docFile.RelativeSourceFileDirectory = fileInfo!.Directory!.FullName.Replace(rootdirectory, "");
                            if (docFile.RelativeSourceFileDirectory.StartsWith('\\'))
                            {
                                docFile.RelativeSourceFileDirectory = docFile.RelativeSourceFileDirectory.Substring(1);
                            }

                            processedFiles.Add(docFile);

                            Debug.WriteLine(docFile.RelativeSourceFilePath);
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
