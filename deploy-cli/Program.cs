using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.Json;

namespace MyCliApp
{
    class Program
    {
        static void Main(string[] args)
        {
            // Cargar la configuración desde el archivo config.json
            var config = LoadConfig();

            // Mostrar las opciones al usuario
            Console.WriteLine("Select an option:");
            for (int i = 0; i < config.Projects.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {config.Projects[i].Name} - {config.Projects[i].Path}");
            }
            Console.WriteLine("5. All Projects");

            // Leer la selección del usuario
            if (!int.TryParse(Console.ReadLine(), out int selection) || selection < 1 || selection > 5)
            {
                Console.WriteLine("Invalid selection. Please choose a number between 1 and 5.");
                return;
            }

            // Procesar la selección
            if (selection == 5)
            {
                Console.WriteLine("Selected all projects:");
                foreach (var project in config.Projects)
                {
                    Console.WriteLine($"- {project.Name} - {project.Path}");
                    // Aquí puedes agregar la lógica para procesar todos los proyectos
                    ProcessProject(project);
                }
            }
            else
            {
                var selectedProject = config.Projects[selection - 1];
                Console.WriteLine($"Selected project: {selectedProject.Name} - {selectedProject.Path}");
                // Aquí puedes agregar la lógica para procesar el proyecto seleccionado
                ProcessProject(selectedProject);
            }

            WaitForUserInput();
        }

        static void ProcessProject(Project project)
        {
            Console.WriteLine($"Processing project: {project.Name}");

            // Verificar si la ruta del proyecto existe
            if (!Directory.Exists(project.Path))
            {
                Console.WriteLine($"Error: The path '{project.Path}' does not exist.");
                return;
            }

            // Limpiar la solución
            Console.WriteLine("Cleaning the solution...");
            RunDotNetCommand(project.Path, "clean");

            // Compilar la solución en modo Release
            Console.WriteLine("Building the solution in Release mode...");
            RunDotNetCommand(project.Path, "build --configuration Release");

            // Publicar el proyecto
            Console.WriteLine("Publishing the project...");
            PublishProject(project.Path);
        }

        static void RunDotNetCommand(string workingDirectory, string command)
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
        static void PublishProject(string projectPath)
        {
            // Verificar si la ruta existe
            if (!Directory.Exists(projectPath))
            {
                Console.WriteLine($"Error: The path '{projectPath}' does not exist.");
                return;
            }

            // Buscar el archivo .csproj en la ruta del proyecto
            string[] csprojFiles = Directory.GetFiles(projectPath, "*.csproj", SearchOption.TopDirectoryOnly);
            if (csprojFiles.Length == 0)
            {
                Console.WriteLine($"Error: No .csproj file found in {projectPath}.");
                return;
            }

            // Tomar el primer archivo .csproj encontrado
            string csprojFile = csprojFiles[0];
            Console.WriteLine($"Found project file: {csprojFile}");

            // Definir la carpeta de publicación
            string publishFolder = Path.Combine(Path.GetDirectoryName(csprojFile), "bin", "Release", "net6.0", "publish");

            // Eliminar la carpeta de publicación si existe
            if (Directory.Exists(publishFolder))
            {
                Console.WriteLine($"Deleting existing publish folder: {publishFolder}");
                Directory.Delete(publishFolder, true);
            }

            // Ejecutar el comando de publicación
            string publishCommand = $"publish \"{csprojFile}\" --configuration Release --framework net6.0 --output \"{publishFolder}\"";
            RunDotNetCommand(Path.GetDirectoryName(csprojFile), publishCommand);
        }

        static Config LoadConfig()
        {
            try
            {
                string configJson = File.ReadAllText("config.json");
                return JsonSerializer.Deserialize<Config>(configJson) ?? new Config { Projects = new List<Project>() };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading config: {ex.Message}");
                return new Config { Projects = new List<Project>() };
            }
        }

        static void WaitForUserInput()
        {
            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }
    }

    // Clases para deserializar el archivo config.json
    public class Config
    {
        public List<Project> Projects { get; set; } = new List<Project>();
    }

    public class Project
    {
        public string Name { get; set; } = string.Empty;
        public string Path { get; set; } = string.Empty;
    }
}