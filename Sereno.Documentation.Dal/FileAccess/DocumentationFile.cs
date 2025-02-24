namespace Sereno.Documentation.FileAccess
{
    public class DocumentationFile
    {
        public required FileInfo File { get; set; }

        public string Verantwortlich { get; set; } = string.Empty;
        public string Information { get; set; } = string.Empty;
        public DateTime NächstePrüfung { get; set; }
        public string Typ { get; set; } = string.Empty;



    }
}
