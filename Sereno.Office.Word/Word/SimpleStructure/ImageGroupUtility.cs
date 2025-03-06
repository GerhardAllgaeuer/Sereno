using DocumentFormat.OpenXml.Wordprocessing;
using System.Linq;
using DocumentFormat.OpenXml.Vml;
using DocumentFormat.OpenXml.Packaging;
using Sereno.Office.Word.SimpleStructure;
using DocumentFormat.OpenXml.Drawing.Wordprocessing;

namespace Sereno.Office.Word.Word.SimpleStructure
{
    public class ImageGroupUtility
    {
        public static bool ParagraphContainsAnImage(Paragraph paragraph)
        {
            if (paragraph == null)
                return false;

            // Suche nach Drawing-Elementen, die Bilder enthalten können
            var drawings = paragraph.Descendants<Drawing>();
            if (drawings.Any())
                return true;

            // Suche nach VML-Bildern (älteres Format)
            var pictures = paragraph.Descendants<Shape>();
            if (pictures.Any())
                return true;

            return false;
        }

        /// <summary>
        /// Paragraph Gruppen Objekt erstellen
        /// </summary>
        public static ImageGroup ProcessParagraph(Paragraph paragraph, WordprocessingDocument document, DocumentGroupOptions options)
        {
            ImageGroup result = new ImageGroup();
            
            // Bilder aus Drawing-Elementen extrahieren (neueres Format)
            var drawings = paragraph.Descendants<Drawing>();
            foreach (var drawing in drawings)
            {
                var blipElements = drawing.Descendants<DocumentFormat.OpenXml.Drawing.Blip>();
                foreach (var blip in blipElements)
                {
                    if (blip.Embed != null)
                    {
                        string relationshipId = blip.Embed.Value;
                        var imagePart = document.MainDocumentPart.GetPartById(relationshipId) as ImagePart;
                        if (imagePart != null)
                        {
                            // Größe des Bildes ermitteln
                            var extent = drawing.Descendants<Extent>().FirstOrDefault();
                            int widthPx = 0;
                            int heightPx = 0;
                            double widthCm = 0;
                            double heightCm = 0;
                            double dpiX = 96; // Standard-DPI
                            double dpiY = 96; // Standard-DPI
                            
                            if (extent != null)
                            {
                                // EMUs (English Metric Units) in Pixel und cm umrechnen
                                // 1 cm = 360000 EMUs
                                // 1 Pixel ≈ 9525 EMUs (bei 96 DPI)
                                widthPx = (int)(extent.Cx.Value / 9525);
                                heightPx = (int)(extent.Cy.Value / 9525);
                                widthCm = extent.Cx.Value / 360000.0;
                                heightCm = extent.Cy.Value / 360000.0;
                                
                                // DPI berechnen
                                // 1 inch = 2.54 cm
                                if (widthCm > 0)
                                    dpiX = widthPx / (widthCm / 2.54);
                                if (heightCm > 0)
                                    dpiY = heightPx / (heightCm / 2.54);
                            }
                            
                            // Bilddaten auslesen
                            byte[] imageData;
                            using (var stream = imagePart.GetStream())
                            {
                                imageData = new byte[stream.Length];
                                stream.Read(imageData, 0, (int)stream.Length);
                            }
                            
                            // ImageInfo erstellen und zur Gruppe hinzufügen
                            var imageInfo = new ImageInfo
                            {
                                Data = imageData,
                                PixelWidth = widthPx,
                                PixelHeight = heightPx,
                                WidthCm = widthCm,
                                HeightCm = heightCm,
                                DpiX = dpiX,
                                DpiY = dpiY
                                // ImageName wird später implementiert
                            };
                            
                            result.Images.Add(imageInfo);
                        }
                    }
                }
            }
            
            // VML-Bilder extrahieren (älteres Format)
            var shapes = paragraph.Descendants<Shape>();
            foreach (var shape in shapes)
            {
                var imageData = shape.Descendants<ImageData>().FirstOrDefault();
                if (imageData != null && imageData.RelationshipId != null)
                {
                    string relationshipId = imageData.RelationshipId.Value;
                    var imagePart = document.MainDocumentPart.GetPartById(relationshipId) as ImagePart;
                    if (imagePart != null)
                    {
                        // Größe des Bildes ermitteln
                        int widthPx = 0;
                        int heightPx = 0;
                        double widthCm = 0;
                        double heightCm = 0;
                        double dpiX = 96; // Standard-DPI
                        double dpiY = 96; // Standard-DPI
                        
                        var style = shape.Style?.Value;
                        if (style != null)
                        {
                            // Versuche, Breite und Höhe aus dem Style-Attribut zu extrahieren
                            var styleValues = style.Split(';');
                            foreach (var styleValue in styleValues)
                            {
                                if (styleValue.StartsWith("width:"))
                                {
                                    var widthStr = styleValue.Substring(6).Trim();
                                    if (widthStr.EndsWith("pt"))
                                    {
                                        float pt;
                                        if (float.TryParse(widthStr.Substring(0, widthStr.Length - 2), out pt))
                                        {
                                            widthPx = (int)(pt * 1.33); // Ungefähre Umrechnung von pt zu px
                                            widthCm = pt / 28.35; // 1 pt = 1/72 inch, 1 inch = 2.54 cm
                                        }
                                    }
                                }
                                else if (styleValue.StartsWith("height:"))
                                {
                                    var heightStr = styleValue.Substring(7).Trim();
                                    if (heightStr.EndsWith("pt"))
                                    {
                                        float pt;
                                        if (float.TryParse(heightStr.Substring(0, heightStr.Length - 2), out pt))
                                        {
                                            heightPx = (int)(pt * 1.33); // Ungefähre Umrechnung von pt zu px
                                            heightCm = pt / 28.35; // 1 pt = 1/72 inch, 1 inch = 2.54 cm
                                        }
                                    }
                                }
                            }
                            
                            // DPI berechnen
                            // 1 inch = 2.54 cm
                            if (widthCm > 0)
                                dpiX = widthPx / (widthCm / 2.54);
                            if (heightCm > 0)
                                dpiY = heightPx / (heightCm / 2.54);
                        }
                        
                        // Bilddaten auslesen
                        byte[] imageBytes;
                        using (var stream = imagePart.GetStream())
                        {
                            imageBytes = new byte[stream.Length];
                            stream.Read(imageBytes, 0, (int)stream.Length);
                        }
                        
                        // ImageInfo erstellen und zur Gruppe hinzufügen
                        var imageInfo = new ImageInfo
                        {
                            Data = imageBytes,
                            PixelWidth = widthPx,
                            PixelHeight = heightPx,
                            WidthCm = widthCm,
                            HeightCm = heightCm,
                            DpiX = dpiX,
                            DpiY = dpiY
                            // ImageName wird später implementiert
                        };
                        
                        result.Images.Add(imageInfo);
                    }
                }
            }
            
            return result;
        }
    }
}
