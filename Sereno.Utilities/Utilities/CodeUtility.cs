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
    }
}
