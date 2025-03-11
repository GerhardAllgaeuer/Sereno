namespace Sereno.Documentation.Synchronization
{
    public class SyncOptions
    {
        public required DirectoryInfo DocumentsDirectory { get; set; }

        public required DirectoryInfo HtmlExportDirectory { get; set; }

        public required string DatabaseConnectionString { get; set; }



    }
}
