using System;

namespace Sereno.Office.Word.SimpleStructure
{
    public class ImageInfo
    {
        /// <summary>
        /// Bilddaten als Byte-Array
        /// </summary>
        public byte[] Data { get; set; }

        /// <summary>
        /// Breite des Bildes in Pixeln
        /// </summary>
        public int PixelWidth { get; set; }

        /// <summary>
        /// Höhe des Bildes in Pixeln
        /// </summary>
        public int PixelHeight { get; set; }

        /// <summary>
        /// Breite des Bildes in Zentimetern
        /// </summary>
        public double WidthCm { get; set; }

        /// <summary>
        /// Höhe des Bildes in Zentimetern
        /// </summary>
        public double HeightCm { get; set; }

        /// <summary>
        /// Horizontale Auflösung in DPI (Dots Per Inch)
        /// </summary>
        public double DpiX { get; set; }

        /// <summary>
        /// Vertikale Auflösung in DPI (Dots Per Inch)
        /// </summary>
        public double DpiY { get; set; }

        /// <summary>
        /// Name des Bildes (wird später implementiert)
        /// </summary>
        public string ImageName { get; set; }
    }
} 