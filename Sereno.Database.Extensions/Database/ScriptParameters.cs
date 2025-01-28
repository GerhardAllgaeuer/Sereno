namespace Sereno.Database
{
    public class ScriptParameters
    {
        public required string ServerName { get; set; }

        public required string ScriptPath { get; set; }

        public required string DatabaseName { get; set; }

        public required string UserName { get; set; }

        public required string Password { get; set; }
    }
}
