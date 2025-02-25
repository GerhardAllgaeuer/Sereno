namespace Sereno.Documentation.FileAccess
{
    public class DocumentationFile
    {
        public required FileInfo File { get; set; }

        public string RelativePath { get; set; } = string.Empty;
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
