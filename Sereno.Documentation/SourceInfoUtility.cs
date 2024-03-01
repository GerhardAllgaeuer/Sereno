using DocumentFormat.OpenXml.Wordprocessing;
using Sereno.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

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
        public static SourceInfo? GetSourceInfo(string directory)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(directory);

            SourceInfoOptions options = new SourceInfoOptions();
            options.Root = directoryInfo.FullName;
            options.ExcludeDirectories = GetExcludeDirectories();
            options.ExcludeWildcardDirectories = GetExcludeLikeDirectories();

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
        /// Verzeichniss, die nicht ausgelesen werden
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
        /// Verzeichniss, die nicht ausgelesen werden
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
        /// Informationen aus dem Verzeichnis auslesen
        /// </summary>
        static SourceInfo? ProcessDirectory(DirectoryInfo dirInfo, SourceInfoOptions options, int level)
        {
            if (StringUtility.IsExcluded(dirInfo.Name, options.ExcludeDirectories, options.ExcludeWildcardDirectories))
                return null;

            SourceInfo sourceInfo = GetSourceInfoFromDirectory(dirInfo, options, level);

            // Durchlaufen von Unterverzeichnissen
            foreach (DirectoryInfo subdir in dirInfo.GetDirectories())
            {
                SourceInfo? childSourceInfo = ProcessDirectory(subdir, options, level++);

                if (childSourceInfo != null)
                    sourceInfo.Children.Add(childSourceInfo);
            }

            return sourceInfo;
        }

    }
}
