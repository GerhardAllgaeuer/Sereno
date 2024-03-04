using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sereno.Documentation
{

    /// <summary>
    /// Optionen für das Auslesen der Quellcode-Informationen
    /// </summary>
    public class SourceInfoOptions
    {

        /// <summary>
        /// Root-Verzeichnis, für die Quellcode-Informationen
        /// </summary>
        public string? Root { get; set; } = null;


        /// <summary>
        /// Typen, die ausgelesen werden
        /// </summary>
        public Dictionary<SourceInfoTypes, string?> Types { get; set; } = [];


        /// <summary>
        /// Verzeichnisse, die nicht ausgelesen werden
        /// </summary>
        public Dictionary<string, string> ExcludeDirectories { get; set; } = [];


        /// <summary>
        /// Verzeichnisse, die nicht ausgelesen werden, basierend auf einem Wildcard-Muster
        /// </summary>
        public Dictionary<string, string> ExcludeWildcardDirectories { get; set; } = [];



        /// <summary>
        /// Dateien, die nicht ausgelesen werden
        /// </summary>
        public Dictionary<string, string> ExcludeFiles { get; set; } = [];


        /// <summary>
        /// Dateien, die nicht ausgelesen werden, basierend auf einem Wildcard-Muster
        /// </summary>
        public Dictionary<string, string> ExcludeWildcardFiles { get; set; } = [];

    }
}
