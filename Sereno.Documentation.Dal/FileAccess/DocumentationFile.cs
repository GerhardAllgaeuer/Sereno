using Sereno.Office.Word.SimpleStructure;
using Sereno.Utilities;
using System.IO;

namespace Sereno.Documentation.FileAccess
{
    public class DocumentationFile
    {
        public required FileInfo File { get; set; }

        public string? Path => this.File?.FullName;

        public string DocumentKey
        {
            get
            {
                string result = FileUtility.RemoveExtension(this.RelativeSourceFilePath);

                return result;
            }
        }

        public string Title { get; set; } = string.Empty;

        public string PlainText { get; set; } = string.Empty;

        public bool HasDocumentationData { get; set; }
        public string RelativeSourceFilePath { get; set; } = string.Empty;
        public string RelativeSourceFileDirectory { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string InfoReceivers { get; set; } = string.Empty;
        public DateTime? NextCheck { get; set; }
        public string Type { get; set; } = string.Empty;


        public List<DocumentGroup> Contents { get; set; } = [];

        public override string ToString()
        {
            return this.RelativeSourceFilePath;
        }



    }
}
