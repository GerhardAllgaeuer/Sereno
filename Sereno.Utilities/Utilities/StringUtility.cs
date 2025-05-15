using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Sereno.Utilities
{
    public class StringUtility
    {
        public static string RemoveLastLineBreak(string value)
        {
            if (string.IsNullOrEmpty(value))
                return value;

            // Entferne den letzten Zeilenumbruch, falls vorhanden
            string trimmedValue = value.TrimEnd('\r', '\n');
            return trimmedValue;
        }



        /// <summary>
        /// Anonymisieren in der Form von M*******
        /// </summary>
        public static string AnonymizeString(string original)
        {
            string anonymized = "";

            if (!string.IsNullOrWhiteSpace(original))
            {
                char anonymizedFirst = original[0];  // Nur den ersten Buchstaben des Vornamens behalten
                anonymized = new string('*', original.Length); // Den Rest maskieren

                anonymized = $"{anonymizedFirst} {anonymized}";
            }

            return anonymized;
        }


        public static string RemoveLastCharacter(string value, string character)
        {
            if (string.IsNullOrEmpty(value))
                return value;

            // Rückwärts maximal 10 Zeichen vom Ende durchlaufen
            int startIndex = Math.Max(0, value.Length - 10);

            // Teilstring von den letzten 10 Zeichen
            string lastPart = value.Substring(startIndex);

            // Position des letzten Kommas in diesem Teilstring
            int lastCommaIndex = lastPart.LastIndexOf(character);

            if (lastCommaIndex >= 0)
            {
                // Absolute Position des Kommas im ursprünglichen String
                int commaPosition = startIndex + lastCommaIndex;

                // Entferne das letzte Komma
                return value.Remove(commaPosition, 1);
            }

            // Kein Komma gefunden, unverändert zurückgeben
            return value;
        }



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
        public static string ConvertWildcardToRegex(string pattern)
        {
            if (String.IsNullOrWhiteSpace(pattern))
                return "";

            string escapedPattern = Regex.Escape(pattern).Replace("\\*", ".*");

            return "^" + escapedPattern + "$";
        }


        /// <summary>
        /// Überprüft, ob ein Eingabestring zu einem Wildcard-Muster passt
        /// </summary>
        public static bool MatchesWildCardPattern(string input, string wildcardPattern)
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
