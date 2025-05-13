using System.Text.Json.Serialization;

namespace Sereno.System.Api.Config
{
    public class UserSettings
    {
        [JsonPropertyName("Pepper")]
        public string Pepper { get; set; } = "";
    }
}
