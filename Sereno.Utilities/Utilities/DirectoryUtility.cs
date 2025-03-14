using System;
using System.IO;

namespace Sereno.Utilities
{
    public class DirectoryUtility
    {

        public static void EnsureEmptyDirectory(DirectoryInfo directory)
        {
            EnsureDirectory(directory);
            CleanUpDirectory(directory);
        }


        public static void EnsureDirectory(DirectoryInfo directory)
        {
            if (!directory.Exists)
            {
                directory.Create();
            }
        }

        public static void CleanUpDirectory(DirectoryInfo rootDirectory)
        {
            if (!Directory.Exists(rootDirectory.FullName))
            {
                return;
            }

            // Lösche alle Dateien im Verzeichnis
            foreach (string file in Directory.GetFiles(rootDirectory.FullName))
            {
                try
                {
                    File.Delete(file);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            // Lösche alle Unterverzeichnisse rekursiv
            foreach (string directory in Directory.GetDirectories(rootDirectory.FullName))
            {
                try
                {
                    Directory.Delete(directory, recursive: true);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
    }
}
