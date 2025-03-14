using System.IO;

namespace Sereno.Utilities
{
    public class FileUtility
    {

        /// <summary>
        /// Extension entfernen, kann auch mit einem relativen Dateinamen umgehen
        /// </summary>
        public static string RemoveExtension(string relativePath)
        {
            // Entfernt die Dateiendung, gibt aber den Rest des Pfads inkl. Verzeichnis zurück
            return Path.Combine(
                Path.GetDirectoryName(relativePath) ?? string.Empty, // Verzeichnis (falls vorhanden)
                Path.GetFileNameWithoutExtension(relativePath)      // Dateiname ohne Extension
            );
        }
    }
}
