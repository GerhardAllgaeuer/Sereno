using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json.Serialization;

namespace Sereno.System.Api.Config
{
    public class IdentitySettings
    {
        [JsonPropertyName("Issuer")]
        public string Issuer { get; set; } = "";

        [JsonPropertyName("Audience")]
        public string Audience { get; set; } = "";

        [JsonPropertyName("TokenLifeTime")]
        public TimeSpan TokenLifeTime { get; set; } = new TimeSpan(1, 0, 0, 0);

        [JsonPropertyName("SecretKey")]
        public string SecretKey { get; set; } = "";


        public SymmetricSecurityKey GetSecurityKey()
        {
            return new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this.SecretKey ?? ""));

        }
    }
}
