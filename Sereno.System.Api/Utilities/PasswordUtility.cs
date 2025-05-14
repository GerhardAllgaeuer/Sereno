namespace Sereno.System.Api.Utilities
{
    public static class PasswordUtility
    {
        public static string HashPassword(string plainPassword, string pepper = "")
        {
            var combined = plainPassword + pepper;
            return BCrypt.Net.BCrypt.HashPassword(combined); // Internes Salt wird automatisch erzeugt
        }

        public static bool VerifyPassword(string plainPassword, string storedHash, string pepper = "")
        {
            var combined = plainPassword + pepper;
            return BCrypt.Net.BCrypt.Verify(combined, storedHash);
        }
    }
}
