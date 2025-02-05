using System;
using System.Diagnostics;
using System.IO;

namespace deploy_cli.Utils
{
    public static class NetUtils
    {
        public static void RunCommand(string command, string arguments, string workingDirectory = "")
        {
            ProcessStartInfo processStartInfo = new ()
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
                    Console.WriteLine($"Error: No se pudo iniciar el proceso '{command}'.");
                    return;
                }

                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();

                process.WaitForExit();

                if (!string.IsNullOrEmpty(output)) Console.WriteLine(output);
                if (!string.IsNullOrEmpty(error)) Console.WriteLine($"Error:\n{error}");

                Console.WriteLine($"Comando finalizado con código {process.ExitCode}.");
            }
        }

        private static bool IsNetFramework(string csprojFile)
        {
            string content = File.ReadAllText(csprojFile);
            return content.Contains("<TargetFrameworkVersion>v4.8</TargetFrameworkVersion>");
        }

        public static void PublishProject(string projectPath)
        {
            if (!Directory.Exists(projectPath))
            {
                Console.WriteLine($"Error: La ruta '{projectPath}' no existe.");
                return;
            }

            string[] csprojFiles = Directory.GetFiles(projectPath, "*.csproj", SearchOption.TopDirectoryOnly);
            if (csprojFiles.Length == 0)
            {
                Console.WriteLine($"Error: No se encontró ningún archivo .csproj en {projectPath}.");
                return;
            }

            string csprojFile = csprojFiles[0];
            Console.WriteLine($"Archivo de proyecto encontrado: {csprojFile}");

            bool isFramework = IsNetFramework(csprojFile);
            if (isFramework)
            {
                Console.WriteLine("Proyecto .NET Framework detectado. Usando MSBuild...");
                PublishWithMSBuild(csprojFile);
            }
            else
            {
                Console.WriteLine("Proyecto .NET Core/.NET detectado. Usando dotnet publish...");
                PublishWithDotNet(csprojFile);
            }
        }

        private static void PublishWithMSBuild(string csprojFile)
        {
            string msBuildPath = @"C:\Program Files\Microsoft Visual Studio\2022\Community\Msbuild\Current\Bin\MSBuild.exe";

            if (!File.Exists(msBuildPath))
            {
                Console.WriteLine("Error: MSBuild no encontrado. Asegúrate de que Visual Studio está instalado.");
                return;
            }

            if (csprojFile == null)
            {
                Console.WriteLine("Error: No se pudo determinar el directorio del proyecto.");
                return;
            }

            string? projectDirectory = Path.GetDirectoryName(csprojFile);
            if (projectDirectory == null)
            {
                Console.WriteLine("Error: No se pudo determinar el directorio del proyecto.");
                return;
            }

            string publishDir = Path.Combine(projectDirectory, "publish");
            string arguments = $"\"{csprojFile}\" /p:Configuration=Release /p:DeployOnBuild=true /p:PublishDir=\"{publishDir}\"";

            RunCommand(msBuildPath, arguments);
        }

        private static void PublishWithDotNet(string csprojFile)
        {
            if (string.IsNullOrEmpty(csprojFile))
            {
                Console.WriteLine("Error: El archivo .csproj no puede ser nulo o vacío.");
                return;
            }

            string? projectDirectory = Path.GetDirectoryName(csprojFile);
            if (projectDirectory == null)
            {
                Console.WriteLine("Error: No se pudo determinar el directorio del proyecto.");
                return;
            }

            string publishFolder = Path.Combine(projectDirectory, "bin", "Release", "publish");
            string arguments = $"publish \"{csprojFile}\" --configuration Release --output \"{publishFolder}\"";

            RunCommand("dotnet", arguments);
        }
    }
}
