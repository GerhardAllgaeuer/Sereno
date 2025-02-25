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


        private static void ProcessDirectory(string rootdirectory, string directory, List<DocumentationFile> processedFiles)
        {
            foreach (string file in Directory.GetFiles(directory))
            {
                try
                {
                    DocumentationFile docFile = DocumentationFileReader.Read(file);

                    docFile.RelativePath = file.Replace(rootdirectory, "");
                    if (docFile.RelativePath.StartsWith(@"\"))
                    {
                        docFile.RelativePath = docFile.RelativePath.Substring(1);
                    }

                    processedFiles.Add(docFile);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error processing file {file}: {ex.Message}");
                }
            }

            foreach (string subDirectory in Directory.GetDirectories(directory))
            {
                ProcessDirectory(rootdirectory, subDirectory, processedFiles);
            }
        }
    }
}
