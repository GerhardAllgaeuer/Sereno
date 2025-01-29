using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sereno.Database
{
    public class ScriptParameters
    {
        public required string ServerName { get; set; }



        public string? ScriptPath { get; set; }

        public string? ScriptContent { get; set; }



        public required string DatabaseName { get; set; }

        public required string UserName { get; set; }

        public required string Password { get; set; }
    }


    public class ScriptUtility
    {
        public static void ExecuteDatabaseScript(ScriptParameters parameters)
        {
            string? sriptPath = parameters.ScriptPath;
            bool isTempFile = false;

            try
            {

                if (!String.IsNullOrWhiteSpace(parameters.ScriptContent))
                {
                    // Wenn nur der Content vom File geliefert wird
                    // und noch keine Datei angelegt worden st
                    if (!String.IsNullOrWhiteSpace(parameters.ScriptPath))
                    {
                        sriptPath = parameters.ScriptPath;
                    }
                    else
                    {
                        sriptPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName() + ".sql");
                    }

                    File.WriteAllText(sriptPath, parameters.ScriptContent);
                    isTempFile = true;

                }
                else
                {
                    // wenn eine Script Datei existiert
                    sriptPath = parameters.ScriptPath;
                }


                Process process = new();

                process.StartInfo.FileName = "sqlcmd";
                process.StartInfo.Arguments = $@"-S {parameters.ServerName} -d {parameters.DatabaseName} -U {parameters.UserName} -P {parameters.Password} -i ""{sriptPath}""";
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.UseShellExecute = false;

                process.Start();

                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();

                process.WaitForExit();

                if (process.ExitCode != 0)
                {
                    throw new Exception($"{error}");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Fehler: {ex.Message}");
            }
            finally
            {
                if (isTempFile)
                {
                    // Nur temporär angelegte Dateien werden danach wieder gelöscht
                    if (File.Exists(sriptPath))
                    {
                        try
                        {
                            File.Delete(sriptPath);
                        }
                        catch (IOException ex)
                        {
                            Console.WriteLine($"Warnung: Konnte temporäre Datei nicht löschen: {ex.Message}");
                        }
                    }
                }
            }
        }
    }
}
