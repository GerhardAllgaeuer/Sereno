using System.IO;

namespace Sereno.Office.Word.Word.SimpleStructure.Converter
{
    public class HtmlConverterOptions
    {
        /// <summary>
        /// Relativer Pfad, in den die Files gespeichert werden (ohne \ am Anfang und Ende)
        /// </summary>
        public string RelativeImageDirectory { get; set; }

        /// <summary>
        /// Relativer Pfad, der vor jedes Bild im Html Dokument gesetzt wird (ohne / am Anfang und Ende)
        /// </summary>
        public string RelativeImageHtmlDirectory { get; set; }

        /// <summary>
        /// Root Verzeichnis, in welches exportiert wird
        /// </summary>
        public DirectoryInfo ExportRootDirectory { get; set; }


        /// <summary>
        /// CSS Datei, die ins Dokument geschrieben wird
        /// </summary>
        public string CssFileName { get; set; } = "styles.css";


        /// <summary>
        /// Dateiname vom Html File
        /// </summary>
        public string HtmlFileName { get; set; } = "document.html";
    }
}
