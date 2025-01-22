using Sereno.Utilities;
using System.IO;
using System.Runtime;
using System.Text;

namespace Sereno.Documentation
{

    /// <summary>
    /// Hilfsklasse für das Auslesen von Quellcode-Informationen
    /// </summary>
    public class SourceInfoUtility
    {

        /// <summary>
        /// Auslesen von Quellcode-Informationen
        /// </summary>
        public static SourceInfo? GetSourceInfo(SourceInfoOptions options)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(options.Root!);

            options.ExcludeDirectories = GetExcludeDirectories();
            options.ExcludeWildcardDirectories = GetExcludeLikeDirectories();
            options.ExcludeFiles = GetExcludeFiles();
            options.ExcludeWildcardFiles = GetExcludeLikeFiles();

            SourceInfo? sourceInfo = ProcessDirectory(directoryInfo, options, 0);

            return sourceInfo;
        }

        public static List<SourceInfo> Flatten(SourceInfo sourceInfo, bool removeRoot = false)
        {
            List<SourceInfo> result = [];

            FlattenInternal(sourceInfo, result);

            if (removeRoot)
                result.RemoveAt(0);

            return result;
        }

        static void FlattenInternal(SourceInfo sourceInfo, List<SourceInfo> result)
        {
            result.Add(sourceInfo);

            foreach (var child in sourceInfo.Children)
            {
                FlattenInternal(child, result);
            }
        }



        /// <summary>
        /// Dateien, die nicht ausgelesen werden
        /// </summary>
        static Dictionary<string, string> GetExcludeFiles()
        {
            Dictionary<string, string> directoryExcludes = new Dictionary<string, string>
            {
            };

            return directoryExcludes;
        }

        /// <summary>
        /// Dateien, die nicht ausgelesen werden
        /// </summary>
        static Dictionary<string, string> GetExcludeLikeFiles()
        {
            Dictionary<string, string> directoryExcludes = new Dictionary<string, string>
            {
                { ".*", "Hidden Files" },
                { "*.jpg", "Bilder" },
                { "*.png", "Bilder" },
                { "*.glb", "3D Dateien" },
                { "*.ico", "Icons" },
                { "*.esproj", "Icons" },
                { "*.eot", "Schriften" },
                { "*.ttf", "Schriften" },
                { "*.woff", "Schriften" },
                { "*.woff2", "Schriften" },
            };

            return directoryExcludes;
        }

        /// <summary>
        /// Verzeichnisse, die nicht ausgelesen werden
        /// </summary>
        static Dictionary<string, string> GetExcludeDirectories()
        {
            Dictionary<string, string> directoryExcludes = new Dictionary<string, string>
            {
                { ".git", "GIT" },
                { ".vs", "Visual Studio" },
                { "dist", "Visual Studio" },
                { "node_modules", "Node" },
                { "obj", "c# Objects" },
                { "bin", "c# Binaries" },
                { "Sereno.Documentation", "Dokumentation" },
                { ".*", "Dokumentation" },
            };

            return directoryExcludes;
        }

        /// <summary>
        /// Verzeichnisse, die nicht ausgelesen werden
        /// </summary>
        static Dictionary<string, string> GetExcludeLikeDirectories()
        {
            Dictionary<string, string> directoryExcludes = new Dictionary<string, string>
            {
                { ".*", "Hidden Folders" },
            };

            return directoryExcludes;
        }



        /// <summary>
        /// Verzeichnis verarbeiten
        /// </summary>
        static SourceInfo GetSourceInfoFromDirectory(DirectoryInfo directory, SourceInfoOptions options, int level)
        {
            SourceInfo sourceInfo = new SourceInfo()
            {
                Location = options.Root == null ? "" : directory.FullName.Replace(options.Root, ""),
                AbsoluteLocation = directory.FullName,
                Type = SourceInfoTypes.Directory,
                Level = level,
            };

            GetDescriptionForDirectory(sourceInfo, "Folder");
            GetDescriptionForDirectory(sourceInfo, "Project");

            // Entfernen des führenden Backslash
            if (sourceInfo.Location.StartsWith('\\'))
                sourceInfo.Location = sourceInfo.Location.Substring(1);

            return sourceInfo;
        }


        static void GetDescriptionForDirectory(SourceInfo sourceInfo, string descriptionFileName)
        {
            if (sourceInfo == null)
                return;

            if (String.IsNullOrWhiteSpace(sourceInfo.AbsoluteLocation))
                return;


            FileInfo documentation = new FileInfo(Path.Combine(sourceInfo.AbsoluteLocation, $"{descriptionFileName}.md"));
            if (documentation.Exists)
            {
                string content = File.ReadAllText(documentation.FullName, Encoding.UTF8);
                sourceInfo.Description = MarkdownUtility.FindSectionText(content, "Kurzbeschreibung");
            }
        }

        /// <summary>
        /// Verzeichnis verarbeiten
        /// </summary>
        static SourceInfo? GetSourceInfoFromFile(FileInfo file, SourceInfoOptions options, int level)
        {
            if (StringUtility.IsExcluded(file.Name, options.ExcludeFiles, options.ExcludeWildcardFiles))
                return null;

            SourceInfo sourceInfo = new()
            {
                Location = options.Root == null ? "" : file.FullName.Replace(options.Root, ""),
                AbsoluteLocation = file.FullName,
                Type = SourceInfoTypes.File,
                Level = level,
            };

            // Entfernen des führenden Backslash
            if (sourceInfo.Location.StartsWith('\\'))
                sourceInfo.Location = sourceInfo.Location.Substring(1);

            return sourceInfo;
        }



        /// <summary>
        /// Informationen aus dem Verzeichnis auslesen
        /// </summary>
        static SourceInfo? ProcessDirectory(DirectoryInfo directory, SourceInfoOptions options, int level)
        {
            if (StringUtility.IsExcluded(directory.Name, options.ExcludeDirectories, options.ExcludeWildcardDirectories))
                return null;

            SourceInfo sourceInfo = GetSourceInfoFromDirectory(directory, options, level);

            if (options.Types.ContainsKey(SourceInfoTypes.File))
            {
                // Durchlaufen der Dateien
                foreach (FileInfo file in directory.GetFiles())
                {
                    SourceInfo? childSourceInfo = GetSourceInfoFromFile(file, options, level++);

                    if (childSourceInfo != null)
                        sourceInfo.Children.Add(childSourceInfo);
                }
            }


            // Durchlaufen von Unterverzeichnissen
            foreach (DirectoryInfo subdir in directory.GetDirectories())
            {
                SourceInfo? childSourceInfo = ProcessDirectory(subdir, options, level++);

                if (childSourceInfo != null)
                    sourceInfo.Children.Add(childSourceInfo);
            }

            return sourceInfo;
        }

    }
}
