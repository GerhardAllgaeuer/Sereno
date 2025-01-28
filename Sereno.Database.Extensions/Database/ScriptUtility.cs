using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sereno.Database
{
    public class ScriptUtility
    {
        public static void ExecuteDatabaseScript(ScriptParameters parameters)
        {
            try
            {
                Process process = new();

                process.StartInfo.FileName = "sqlcmd";
                process.StartInfo.Arguments = $@"-S {parameters.ServerName} -d {parameters.DatabaseName} -U {parameters.UserName} -P {parameters.Password} -i ""{parameters.ScriptPath}""";
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
        }

    }
}
