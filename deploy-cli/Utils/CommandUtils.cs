using System.Diagnostics;

namespace deploy_cli.Utils
{
    public class CommandUtils
    {
        public static void RunPowerShellScript(string scriptPath)
        {
            if (!File.Exists(scriptPath))
            {
                throw new FileNotFoundException($"Error: Script file not found at {scriptPath}.");
            }

            RunPowerShellCommand(null, scriptPath);
        }

        public static void RunPowerShellCommand(string? script, string? file)
        {
            ProcessStartInfo processStartInfo;

            if (string.IsNullOrEmpty(file))
            {
                processStartInfo = new ProcessStartInfo()
                {
                    FileName = "powershell.exe",
                    Arguments = $"-NoProfile -ExecutionPolicy Bypass -Command \"{script}\"",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };
            }
            else if (string.IsNullOrEmpty(script))
            {
                processStartInfo = new ProcessStartInfo()
                {
                    FileName = "powershell.exe",
                    Arguments = $"-NoProfile -ExecutionPolicy Bypass -File \"{file}\"",
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
                    throw new InvalidOperationException("Error: Unable to start PowerShell process.");
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
                    throw new InvalidOperationException($"PowerShell Error: {error}");
                }

                if (process.ExitCode != 0)
                {
                    throw new InvalidOperationException($"PowerShell command failed with exit code {process.ExitCode}.");
                }
                else
                {
                    Console.WriteLine("PowerShell command executed successfully.");
                }
            }
        }
    }
}
