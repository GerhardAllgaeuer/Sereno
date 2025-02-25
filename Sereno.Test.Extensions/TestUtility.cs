namespace Sereno.Test
{
    public class TestUtility
    {

        /// <summary>
        /// Root Verzeichnis des Projekts auslesen
        /// </summary>
        public static DirectoryInfo GetProjectRoot()
        {
            string currentDirectory = AppContext.BaseDirectory;

            DirectoryInfo directory = new DirectoryInfo(currentDirectory);

            if (directory.Name.StartsWith(@"net"))
                directory = directory.Parent!;

            if (directory.Name.Contains("Debug"))
                directory = directory.Parent!;

            if (directory.Name.Contains("bin"))
                directory = directory.Parent!;

            directory = directory.Parent!;

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
