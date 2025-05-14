using System.Text.Json;

namespace Sereno.System.Api.Config
{
    public class AppConfigService
    {
        private readonly string configPath;
        private readonly JsonSerializerOptions jsonOptions = new(JsonSerializerDefaults.Web)
        {
            WriteIndented = true
        };

        public AppConfigService(string configPath)
        {
            this.configPath = configPath;
        }

        public AppConfig Load()
        {
            if (!File.Exists(configPath))
            {
                var defaultConfig = CreateDefaultConfig();
                Save(defaultConfig);
                return defaultConfig;
            }

            var json = File.ReadAllText(configPath);
            return JsonSerializer.Deserialize<AppConfig>(json, jsonOptions) ?? CreateDefaultConfig();

        }

        public void Save(AppConfig config)
        {
            var json = JsonSerializer.Serialize(config, jsonOptions);
            File.WriteAllText(configPath, json);
        }

        private AppConfig CreateDefaultConfig()
        {
            return new AppConfig
            {
                ApplicationUrl = "https://localhost:7092",
                IdentitySettings = new IdentitySettings()
                {
                    Audience = "connexia-api",
                    Issuer = "https://auth.connexia.at",
                    TokenLifeTime = new TimeSpan(1, 0, 0, 0),
                    SecretKey = "3094-0337-10484-2024-26941-30374-031",
                },
                UserSettings = new UserSettings()
                {
                    Pepper = "034-0349-03489-72349"
                },
            };
        }
    }

}
