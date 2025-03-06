using System.IO;

namespace Sereno.Documentation.FileAccess
{
    public class DocumentationFile
    {
        public required FileInfo File { get; set; }

        public string? Path => this.File?.FullName;

        public string Key
        {
            get
            {
                string result = "";
                if (this.File != null)
                {
                    result = System.IO.Path.GetFileNameWithoutExtension(this.File.Name);
                }

                return result;
            }
        }

        public string Title { get; set; } = string.Empty;


        public bool HasDocumentationData { get; set; }
        public string RelativePath { get; set; } = string.Empty;
        public string RelativeDirectory { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string InfoReceivers { get; set; } = string.Empty;
        public DateTime? NextCheck { get; set; }
        public string Type { get; set; } = string.Empty;


        public override string ToString()
        {
            return this.RelativePath;
        }



    }
}
