using Sereno.Logging;
using Sereno.System.Api.Config;
using System.Text.Json;

namespace Sereno.System.Api.Utilities
{
    public class UserService
    {

        AppConfig config;

        public UserService(AppConfig config)
        {
            this.config = config;
        }

        private readonly JsonSerializerOptions jsonOptions = new(JsonSerializerDefaults.Web)
        {
            WriteIndented = true
        };
        public void SaveUser(User user, string password)
        {
            var userPath = GetUserFile(user.UserName);
            Directory.CreateDirectory(Path.GetDirectoryName(userPath)!);

            user.Hash = PasswordUtility.HashPassword(password, config.UserSettings.Pepper);

            var json = JsonSerializer.Serialize(user, jsonOptions);
            File.WriteAllText(userPath, json);
        }


        private string GetUserFile(string userName)
        {
            var userPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                    "Connexia",
                    "System.Api",
                    "Users",
                    userName,
                    "UserData.json"
                );

            return userPath;
        }

        public bool VerifyUser(string userName, string password)
        {
            var userPath = GetUserFile(userName);

            if (!File.Exists(userPath))
                return false;

            var json = File.ReadAllText(userPath);
            try
            {
                User? user = JsonSerializer.Deserialize<User>(json, jsonOptions);

                if (user == null)
                    return false;

                return PasswordUtility.VerifyPassword(password, user.Hash, config.UserSettings.Pepper);
            }
            catch (Exception ex)
            {
                Log.Instance.Error(null, ex);
                return false;
            }

        }
    }
}
