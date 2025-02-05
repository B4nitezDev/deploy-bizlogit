using deploy_cli.Entities;
using deploy_cli.Utils;

namespace MyCliApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Config? config = ConfigUtils.LoadConfig();

            Console.WriteLine("Select an option:");
            for (int i = 0; i < config.Projects.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {config.Projects[i].Name} - {config.Projects[i].Path}");
            }
            Console.WriteLine("5. All Projects");

            if (!int.TryParse(Console.ReadLine(), out int selection) || selection < 1 || selection > 5)
            {
                Console.WriteLine("Invalid selection. Please choose a number between 1 and 5.");
                return;
            }

            if (selection == 5)
            {
                Console.WriteLine("Selected all projects:");
                foreach (var project in config.Projects)
                {
                    Console.WriteLine($"- {project.Name} - {project.Path}");
                    ProcessProject(project);
                }
            }
            else
            {
                var selectedProject = config.Projects[selection - 1];
                Console.WriteLine($"Selected project: {selectedProject.Name} - {selectedProject.Path}");
                ProcessProject(selectedProject);
            }

            WaitForUserInput();
        }

        static void ProcessProject(Project project)
        {
            Console.WriteLine($"Processing project: {project.Name}");

            if (!Directory.Exists(project.Path))
            {
                Console.WriteLine($"Error: The path '{project.Path}' does not exist.");
                return;
            }

            Console.WriteLine("Cleaning the solution...");
            NetUtils.RunDotNetCommand(project.Path, "clean");

            // Compilar la solución en modo Release
            Console.WriteLine("Building the solution in Release mode...");
            NetUtils.RunDotNetCommand(project.Path, "build --configuration Release");

            // Publicar el proyecto
            Console.WriteLine("Publishing the project...");
            NetUtils.PublishProject(project.Path);

            // Ejecutar el script de PowerShell si está configurado
            if (!string.IsNullOrEmpty(project.PowerShellScript))
            {
                Console.WriteLine($"Executing PowerShell script: {project.PowerShellScript}");
                CommandUtils.RunPowerShellScript(project.PowerShellScript);
            }
            else
            {
                Console.WriteLine("No PowerShell script configured for this project.");
            }
        }

        static void WaitForUserInput()
        {
            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }
    }
}