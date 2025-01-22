using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sereno.Documentation
{
    public class SourceInfo
    {

        /// <summary>
        /// Pfad zum Ordner / zur Datei
        /// </summary>
        public string? Location { get; set; }


        /// <summary>
        /// Typ des Quell-Code-Objekts
        /// </summary>
        public SourceInfoTypes? Type { get; set; }


        /// <summary>
        /// Absoluter Pfad zum Ordner / zur Datei
        /// </summary>
        public string? AbsoluteLocation { get; internal set; }


        /// <summary>
        /// Beschreibung zum Ordner / zur Datei
        /// </summary>
        public string? Description { get; set; }


        /// <summary>
        /// Level des Ordners in der Source Code Struktur
        /// </summary>
        public int Level { get; set; }


        /// <summary>
        /// Untergeordnete Quellcode-Informationen
        /// </summary>
        public List<SourceInfo> Children { get; set; } = new List<SourceInfo>();

    }
}
