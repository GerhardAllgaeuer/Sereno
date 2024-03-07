using System.Text.RegularExpressions;

namespace Sereno.Utilities
{
    public class StringUtility
    {


        /// <summary>
        /// Entfernt doppelte Leerzeichen, CR/LFs und Tabs aus einer Liste von Strings
        /// </summary>
        public static string CleanAndJoinStringList(List<string> strings, string join)
        {
            return string.Join(join,
                strings
                    .Where(s => !string.IsNullOrWhiteSpace(s)) // Entferne leere Strings
                    .Select(s => Regex.Replace(s.Trim(), @"\s+", " ")) // Entferne doppelte Leerzeichen, cr/lfs und tabs
                    .Select(s => s.Replace("\n", "").Replace("\r", "").Replace("\t", "")) // Alternativ zum Regex, explizites Entfernen von CR/LFs und Tabs
                    .Distinct() // Entferne Duplikate
            );
        }



        /// <summary>
        /// Konvertiert ein Wildcard-Muster in ein Regex-Muster
        /// </summary>
        public static string ConvertWildcardToRegex(string? pattern)
        {
            if (String.IsNullOrWhiteSpace(pattern))
                return "";

            string escapedPattern = Regex.Escape(pattern).Replace("\\*", ".*");

            return "^" + escapedPattern + "$";
        }


        /// <summary>
        /// Überprüft, ob ein Eingabestring zu einem Wildcard-Muster passt
        /// </summary>
        public static bool MatchesWildCardPattern(string? input, string? wildcardPattern)
        {
            if (String.IsNullOrWhiteSpace(input))
                input = "";

            if (String.IsNullOrWhiteSpace(wildcardPattern))
                wildcardPattern = "";

            string regexPattern = ConvertWildcardToRegex(wildcardPattern);
            return Regex.IsMatch(input, regexPattern);

        }


        /// <summary>
        /// Prüfung, ob ein String in einer Liste von Ausnahmen enthalten ist.
        /// Es gibt zwei Arten von Ausnahmen, die aus Performancegründen separat übergeben werden:
        /// - Ausnahmen, die exakt übereinstimmen
        /// - Ausnahmen, die Wildcard-Muster enthalten
        /// </summary>
        public static bool IsExcluded(string? text, Dictionary<string, string>? excludes, Dictionary<string, string>? wildcardExcludes = null)
        {
            if (text == null)
                return false;

            if (excludes != null)
            {
                if (excludes.ContainsKey(text))
                    return true;
            }

            if (wildcardExcludes != null)
            {
                foreach (var exclude in wildcardExcludes)
                {
                    if (MatchesWildCardPattern(text, exclude.Key))
                        return true;
                }
            }

            return false;
        }
    }
}
