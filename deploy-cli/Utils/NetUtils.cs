using System.Diagnostics;

namespace deploy_cli.Utils
{
    public static class NetUtils
    {
        public static void RunDotNetCommand(string workingDirectory, string command)
        {
            var processStartInfo = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = command,
                WorkingDirectory = workingDirectory,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (var process = Process.Start(processStartInfo))
            {
                if (process == null)
                {
                    Console.WriteLine("Error: Unable to start the dotnet process.");
                    return;
                }

                // Capturar la salida del comando
                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();

                process.WaitForExit();

                // Mostrar la salida del comando
                if (!string.IsNullOrEmpty(output))
                {
                    Console.WriteLine(output);
                }

                // Mostrar errores si los hay
                if (!string.IsNullOrEmpty(error))
                {
                    Console.WriteLine("Error:");
                    Console.WriteLine(error);
                }

                if (process.ExitCode != 0)
                {
                    Console.WriteLine($"Command failed with exit code {process.ExitCode}.");
                }
                else
                {
                    Console.WriteLine("Command executed successfully.");
                }
            }
        }


        public static void PublishProject(string projectPath)
        {
            if (!Directory.Exists(projectPath))
            {
                Console.WriteLine($"Error: The path '{projectPath}' does not exist.");
                return;
            }

            string[] csprojFiles = Directory.GetFiles(projectPath, "*.csproj", SearchOption.TopDirectoryOnly);
            if (csprojFiles.Length == 0)
            {
                Console.WriteLine($"Error: No .csproj file found in {projectPath}.");
                return;
            }

            string csprojFile = csprojFiles[0];
            Console.WriteLine($"Found project file: {csprojFile}");

            string publishFolder = Path.Combine(Path.GetDirectoryName(csprojFile), "bin", "Release", "net6.0", "publish");

            if (Directory.Exists(publishFolder))
            {
                Console.WriteLine($"Deleting existing publish folder: {publishFolder}");
                Directory.Delete(publishFolder, true);
            }

            string publishCommand = $"publish \"{csprojFile}\" --configuration Release --framework net6.0 --output \"{publishFolder}\"";
            RunDotNetCommand(Path.GetDirectoryName(csprojFile), publishCommand);
        }
    }
}
