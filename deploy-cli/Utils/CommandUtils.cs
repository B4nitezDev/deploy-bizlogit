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

            using (var process = new Process())
            {
                process.StartInfo = processStartInfo;
                process.OutputDataReceived += (sender, e) => { if (e.Data != null) Console.WriteLine(e.Data); };
                process.ErrorDataReceived += (sender, e) => { if (e.Data != null) Console.WriteLine($"Error: {e.Data}"); };

                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();

                process.WaitForExit();

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

