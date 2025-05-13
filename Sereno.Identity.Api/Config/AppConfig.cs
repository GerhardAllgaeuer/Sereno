using System.Text.Json.Serialization;

namespace Sereno.System.Api.Config
{
    public class AppConfig
    {
        [JsonPropertyName("ApplicationUrl")]
        public string ApplicationUrl { get; set; } = "https://localhost:7092";
        
        [JsonPropertyName("IdentitySettings")]
        public IdentitySettings IdentitySettings { get; set; } = new();

        [JsonPropertyName("UserSettings")]
        public UserSettings UserSettings { get; set; } = new();
    }
}
