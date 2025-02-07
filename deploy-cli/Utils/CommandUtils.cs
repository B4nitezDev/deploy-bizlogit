using System.Diagnostics;

namespace deploy_cli.Utils
{
    public class CommandUtils
    {
        public static void RunPowerShellScript(string scriptPath)
        {
            if (!File.Exists(scriptPath))
            {
                Console.WriteLine($"Error: Script file not found at {scriptPath}.");
                return;
            }

            RunPowerShellCommand(null, scriptPath);
        }

        public static void RunPowerShellCommand(string? script, string? file)
        {
            ProcessStartInfo processStartInfo;

            if (string.IsNullOrEmpty(script))
            {
                processStartInfo = new ProcessStartInfo()
                {
                    FileName = "powershell.exe",
                    Arguments = $"-NoProfile -ExecutionPolicy Bypass \"{script}\"",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

            }
            else if (string.IsNullOrEmpty(file))
            {
                processStartInfo = new ProcessStartInfo()
                {
                    FileName = "powershell.exe",
                    Arguments = $"-NoProfile -ExecutionPolicy Bypass -File \"{script}\"",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

            }
            else
            {
                throw new Exception("Command no valido");
            }

            using (var process = Process.Start(processStartInfo))
            {
                if (process == null)
                {
                    Console.WriteLine("Error: Unable to start PowerShell process.");
                    return;
                }

                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();

                process.WaitForExit();

                if (!string.IsNullOrEmpty(output))
                {
                    Console.WriteLine("PowerShell Output:");
                    Console.WriteLine(output);
                }

                if (!string.IsNullOrEmpty(error))
                {
                    Console.WriteLine("PowerShell Error:");
                    Console.WriteLine(error);
                }

                if (process.ExitCode != 0)
                {
                    Console.WriteLine($"PowerShell command failed with exit code {process.ExitCode}.");
                }
                else
                {
                    Console.WriteLine("PowerShell command executed successfully.");
                }
            }
        }

    }
}
