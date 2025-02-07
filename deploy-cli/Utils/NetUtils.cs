using System;
using System.Diagnostics;
using System.IO;

namespace deploy_cli.Utils
{
    public static class NetUtils
    {
        public static void RunCommand(string command, string arguments, string workingDirectory = "")
        {
            ProcessStartInfo processStartInfo = new()
            {
                FileName = command,
                Arguments = arguments,
                WorkingDirectory = workingDirectory,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (Process? process = Process.Start(processStartInfo))
            {
                if (process == null)
                {
                    throw new InvalidOperationException($"Error: No se pudo iniciar el proceso '{command}'.");
                }

                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();

                process.WaitForExit();

                if (!string.IsNullOrEmpty(output)) Console.WriteLine(output);
                if (!string.IsNullOrEmpty(error)) Console.WriteLine($"Error:\n{error}");

                if (process.ExitCode != 0)
                {
                    throw new InvalidOperationException($"Error: El comando '{command} {arguments}' falló con el código de salida {process.ExitCode}.");
                }

                Console.WriteLine($"Comando finalizado con código {process.ExitCode}.");
            }
        }

        public static void PublishProject(string projectPath, int version)
        {
            if (!Directory.Exists(projectPath))
            {
                throw new DirectoryNotFoundException($"Error: La ruta '{projectPath}' no existe.");
            }

            string[] csprojFiles = Directory.GetFiles(projectPath, "*.csproj", SearchOption.TopDirectoryOnly);
            if (csprojFiles.Length == 0)
            {
                throw new FileNotFoundException($"Error: No se encontró ningún archivo .csproj en {projectPath}.");
            }

            string csprojFile = csprojFiles[0];
            Console.WriteLine($"Archivo de proyecto encontrado: {csprojFile}");

            if (version == 4)
            {
                Console.WriteLine("Proyecto .NET Framework detectado. Usando MSBuild...");
                PublishWithMSBuild(csprojFile);
            }
            else if(version > 4)
            {
                Console.WriteLine("Proyecto .NET Core/.NET detectado. Usando dotnet publish...");
                PublishWithDotNet(csprojFile);
            }
            else
            {
                Console.WriteLine("Version de .Net no soportada");
            }
        }

        private static void PublishWithMSBuild(string csprojFile)
        {
            string msBuildPath = @"C:\Program Files\Microsoft Visual Studio\2022\Community\Msbuild\Current\Bin\MSBuild.exe";

            if (!File.Exists(msBuildPath))
            {
                throw new FileNotFoundException("Error: MSBuild no encontrado. Asegúrate de que Visual Studio está instalado.");
            }

            string? projectDirectory = Path.GetDirectoryName(csprojFile);
            if (string.IsNullOrWhiteSpace(projectDirectory))
            {
                throw new InvalidOperationException("Error: No se pudo determinar el directorio del proyecto.");
            }

            string publishDir = Path.Combine(projectDirectory, "publish");
            string arguments = $"\"{csprojFile}\" /p:Configuration=Release /p:DeployOnBuild=true /p:PublishDir=\"{publishDir}\"";

            RunCommand("MSBuild", $"\"{csprojFile}\" /p:Configuration=Release /p:DeployOnBuild=true", projectDirectory);
        }

        private static void PublishWithDotNet(string csprojFile)
        {
            if (string.IsNullOrWhiteSpace(csprojFile))
            {
                throw new InvalidOperationException("Error: El archivo .csproj no puede ser nulo o vacío.");
            }

            string? projectDirectory = Path.GetDirectoryName(csprojFile);
            if (string.IsNullOrWhiteSpace(projectDirectory))
            {
                throw new InvalidOperationException("Error: No se pudo determinar el directorio del proyecto.");
            }

            string publishFolder = Path.Combine(projectDirectory, "bin", "Release", "publish");
            string arguments = $"publish \"{csprojFile}\" --configuration Release --output \"{publishFolder}\"";

            RunCommand("dotnet", arguments, projectDirectory);
        }
    }
}
