using System;

namespace Sereno
{
    public class Context
    {
        public string UserName { get; set; } = string.Empty;
        public string MachineName { get; set; } = string.Empty;
        public string? IpAddress { get; set; }
        public string? ClientVersion { get; set; }
    }
}
