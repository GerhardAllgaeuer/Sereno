using System.Diagnostics;

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


        private static bool IncludeFile(string file)
        {
            bool result = true;

            if (!file.EndsWith(".docx"))
                result = false;

            return result;
        }


        private static void ProcessDirectory(string rootdirectory, string directory, List<DocumentationFile> processedFiles)
        {
            foreach (string file in Directory.GetFiles(directory))
            {
                if (IncludeFile(file))
                {
                    try
                    {
                        DocumentationFile? docFile = DocumentationFileReader.Read(file);
                        if (docFile != null)
                        {
                            docFile.RelativePath = file.Replace(rootdirectory, "");
                            if (docFile.RelativePath.StartsWith('\\'))
                            {
                                docFile.RelativePath = docFile.RelativePath.Substring(1);
                            }

                            processedFiles.Add(docFile);

                            Debug.WriteLine(docFile.RelativePath);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error processing file {file}: {ex.Message}");
                    }
                }
            }

            foreach (string subDirectory in Directory.GetDirectories(directory))
            {
                ProcessDirectory(rootdirectory, subDirectory, processedFiles);
            }
        }
    }
}
