using deploy_cli.Entities;
using deploy_cli.Utils;

namespace MyCliApp
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Config? config = ConfigUtils.LoadConfig();

                int selection = GetOptions(config);
                ValidateOptions(config, selection);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            finally
            {
                WaitForUserInput();
            }
        }

        static void ProcessProject(Project project)
        {
            try
            {
                Console.WriteLine($"Processing project: {project.Name}");

                if (!Directory.Exists(project.Path))
                    throw new DirectoryNotFoundException($"Error: The path '{project.Path}' does not exist.");

                switch (project.Version)
                {
                    case 4: 
                        ProccessNet4(project);
                        break;
                    case 6:
                        ProccessNet6(project);
                        break;
                    case 100:
                        Mobile(project);
                        break;
                    case 101:
                        Bizion(project);
                        break;
                    default:
                        throw new Exception("Version no soportada");
                }


                if (string.IsNullOrEmpty(project.PowerShellScript))
                {
                    Console.WriteLine("No hay script configurado");
                    return;
                }

                // Ejecutar el script de PowerShell si está configurado
                ProccessPowershelScript(project.PowerShellScript);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing project {project.Name}: {ex.Message}");
                throw;
            }
        }

        static void WaitForUserInput()
        {
            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }

        static int GetOptions(Config config)
        {
            Console.WriteLine("Select an option:");
            for (int i = 0; i < config.Projects.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {config.Projects[i].Name} - {config.Projects[i].Path}");
            }
            Console.WriteLine($"{config.Projects.Count + 1}. All Projects");

            if (!int.TryParse(Console.ReadLine(), out int selection) || selection < 1 || selection > config.Projects.Count + 1)
            {
                Console.WriteLine("Invalid selection. Please choose a number between 1 and 5.");
            }

            return selection;
        }

        static void ValidateOptions(Config config, int selection)
        {
            if (selection == config.Projects.Count + 1)
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
        }

        static void ProccessNet6(Project project)
        {
            // Clean la solucion
            Console.WriteLine("Cleaning the solution...");
            NetUtils.RunCommand("dotnet", "clean", project.Path);

            // Compilar la solución en modo Release
            Console.WriteLine("Building the solution in Release mode...");
            NetUtils.RunCommand("dotnet", "build --configuration Release", project.Path);

            // Publicar el proyecto
            Console.WriteLine("Publishing the project...");
            NetUtils.PublishProject(project.Path.ToString(), project.Version);
        }

        static void ProccessNet4(Project project)
        {
            string msBuildPath = @"C:\Program Files\Microsoft Visual Studio\2022\Community\Msbuild\Current\Bin\MSBuild.exe";

            // Clean la solucion
            Console.WriteLine("Cleaning the solution...");
            NetUtils.RunCommand("dotnet", "clean", project.Path);

            string csprojFile = Directory.GetFiles(project.Path, "*.csproj", SearchOption.TopDirectoryOnly).FirstOrDefault();
            if (csprojFile == null)
                throw new FileNotFoundException($"Error: No se encontró ningún archivo .csproj en {project.Path}.");

            // Compilar la solucion en modo Release
            Console.WriteLine("Building the solution in Release mode...");
            NetUtils.RunCommand(msBuildPath, $"\"{csprojFile}\" /p:Configuration=Release", project.Path);

            // Publicar el proyecto
            Console.WriteLine("Publishing the project...");
            NetUtils.PublishProject(project.Path.ToString(), project.Version);

        }

        static void Mobile(Project project)
        {
            Console.WriteLine("NODE");
            FrontUtil.BuildFrontMobile(project.Path);
        }

        static void Bizion(Project project)
        {
            Console.WriteLine("NODE");
            FrontUtil.BuildFromBizion(project.Path);
        }

        static void ProccessPowershelScript(string path)
        {
            Console.WriteLine($"Executing PowerShell script: {path}");
            CommandUtils.RunPowerShellScript(path);
        }
    }
}