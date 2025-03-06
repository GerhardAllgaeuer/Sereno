using Sereno.Office.Word.SimpleStructure;
using System.Collections.Generic;

namespace Sereno.Office.Word.Word.SimpleStructure
{
    public class ImageGroup : DocumentGroup
    {
        public List<ImageInfo> Images { get; set; } = new List<ImageInfo>();
    }
}
