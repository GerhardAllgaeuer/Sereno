using System;
using System.IO;
using System.Linq;

namespace Sereno.Utilities.DirectorySync
{
    public class DirectorySyncUtility
    {
        public static void SyncDirectories(string sourceDir, string targetDir)
        {
            if (!Directory.Exists(sourceDir))
                throw new DirectoryNotFoundException($"Quellverzeichnis nicht gefunden: {sourceDir}");

            if (!Directory.Exists(targetDir))
                Directory.CreateDirectory(targetDir);

            // Synchronisiere Dateien
            foreach (var sourceFile in Directory.GetFiles(sourceDir, "*", SearchOption.AllDirectories))
            {
                string relativePath = GetRelativePath(sourceDir, sourceFile);
                string targetFile = Path.Combine(targetDir, relativePath);
                string targetFolder = Path.GetDirectoryName(targetFile) ?? string.Empty;

                if (!Directory.Exists(targetFolder))
                    Directory.CreateDirectory(targetFolder);

                if (!File.Exists(targetFile) || File.GetLastWriteTimeUtc(sourceFile) > File.GetLastWriteTimeUtc(targetFile))
                {
                    File.Copy(sourceFile, targetFile, true);
                }
            }

            // Lösche Dateien, die nicht mehr in der Quelle existieren
            foreach (var targetFile in Directory.GetFiles(targetDir, "*", SearchOption.AllDirectories))
            {
                string relativePath = GetRelativePath(targetDir, targetFile);
                string sourceFile = Path.Combine(sourceDir, relativePath);

                if (!File.Exists(sourceFile))
                {
                    File.Delete(targetFile);
                }
            }

            RemoveEmptyDirectories(targetDir);
        }

        public static void RemoveGitFiles(string targetDir)
        {
            foreach (var targetSubDir in Directory.GetDirectories(targetDir, "*", SearchOption.AllDirectories))
            {
                foreach (var gitFile in Directory.GetFiles(targetSubDir, ".git*"))
                {
                    File.Delete(gitFile);
                }
            }
        }

        public static void RemoveBinAndObjDirectory(string targetDir)
        {
            string binDir = Path.Combine(targetDir, "bin");
            if (Directory.Exists(binDir))
            {
                Directory.Delete(binDir, true);
            }

            string objDir = Path.Combine(targetDir, "obj");
            if (Directory.Exists(objDir))
            {
                Directory.Delete(objDir, true);
            }
        }

        private static void RemoveEmptyDirectories(string dir)
        {
            foreach (var subDir in Directory.GetDirectories(dir))
            {
                RemoveEmptyDirectories(subDir);
                if (!Directory.EnumerateFileSystemEntries(subDir).Any())
                {
                    Directory.Delete(subDir);
                }
            }
        }

        private static string GetRelativePath(string basePath, string fullPath)
        {
            Uri baseUri = new Uri(basePath.EndsWith("\\") ? basePath : basePath + "\\");
            Uri fileUri = new Uri(fullPath);
            return Uri.UnescapeDataString(baseUri.MakeRelativeUri(fileUri).ToString()).Replace('/', Path.DirectorySeparatorChar);
        }
    }
}
