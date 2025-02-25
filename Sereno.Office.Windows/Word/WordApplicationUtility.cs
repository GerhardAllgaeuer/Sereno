using System;
using MsWord = Microsoft.Office.Interop.Word;
using System.Text.RegularExpressions;
using Sereno.Utilities;

namespace Sereno.Office.Windows.Word
{
    public class WordApplicationUtility
    {
        public static void TestStandardIntegration()
        {
            Context context = ContextUtility.Create("aljkds");
        }

        public static void CloseWordDocument(string fileNameOrPath)
        {
            MsWord.Application wordApp;
            try
            {
                wordApp = (MsWord.Application)System.Runtime.InteropServices.Marshal.GetActiveObject("Word.Application");
            }
            catch (System.Runtime.InteropServices.COMException)
            {
                Console.WriteLine("MsWord ist nicht geöffnet.");
                return;
            }

            foreach (MsWord.Document doc in wordApp.Documents)
            {
                if (MatchesWildCardPattern(doc.FullName, fileNameOrPath))
                {
                    doc.Close(MsWord.WdSaveOptions.wdSaveChanges); // Schließe das Dokument und speichere Änderungen
                    Console.WriteLine("Dokument geschlossen: " + doc.FullName);
                    break;
                }
                if (doc.FullName == fileNameOrPath)
                {
                    doc.Close(MsWord.WdSaveOptions.wdSaveChanges); // Schließe das Dokument und speichere Änderungen
                    Console.WriteLine("Dokument geschlossen: " + fileNameOrPath);
                    break;
                }
            }

            // Wenn keine weiteren Dokumente geöffnet sind, können Sie MsWord schließen
            if (wordApp.Documents.Count == 0)
            {
                wordApp.Quit();
            }
        }

        /// <summary>
        /// Konvertiert ein Wildcard-Muster in ein Regex-Muster
        /// </summary>
        public static string ConvertWildcardToRegex(string pattern)
        {
            string escapedPattern = Regex.Escape(pattern).Replace("\\*", ".*");

            return "^" + escapedPattern + "$";
        }


        /// <summary>
        /// Überprüft, ob ein Eingabestring zu einem Wildcard-Muster passt
        /// </summary>
        public static bool MatchesWildCardPattern(string input, string wildcardPattern)
        {
            string regexPattern = ConvertWildcardToRegex(wildcardPattern);
            return Regex.IsMatch(input, regexPattern);
        }

    }
}
