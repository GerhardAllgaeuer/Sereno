using System;
using System.IO;

namespace Sereno.Utilities
{
    public class CodeUtility
    {
        public static DirectoryInfo GetSolutionDirectory()
        {
            var currentDirectory = Directory.GetCurrentDirectory();
            var directory = new DirectoryInfo(currentDirectory);

            while (directory != null && directory.GetFiles("*.sln").Length == 0)
            {
                directory = directory.Parent;
            }

            if (directory == null)
            {
                throw new InvalidOperationException("Solution-Verzeichnis konnte nicht gefunden werden.");
            }

            return directory;
        }


        /// <summary>
        /// Root Verzeichnis des Projekts auslesen
        /// </summary>
        public static DirectoryInfo GetProjectRoot()
        {
            string currentDirectory = AppContext.BaseDirectory;

            DirectoryInfo directory = new DirectoryInfo(currentDirectory);

            if (directory.Name.StartsWith(@"net"))
                directory = directory.Parent;

            if (directory.Name.Contains("Debug"))
                directory = directory.Parent;

            if (directory.Name.Contains("bin"))
                directory = directory.Parent;

            directory = directory.Parent;

            return directory;

        }


        /// <summary>
        /// Datenverzeichnis auslesen
        /// </summary>
        public static DirectoryInfo GetDataDirectory()
        {
            return new DirectoryInfo(@"D:\Data");
        }
    }
}
