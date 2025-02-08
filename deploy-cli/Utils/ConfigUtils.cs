using deploy_cli.Entities;
using System.Text.Json;

namespace deploy_cli.Utils
{
    public static class ConfigUtils
    {
        public static Config LoadConfig()
        {
            try
            {
                string configJson = File.ReadAllText("config.json");
                return JsonSerializer.Deserialize<Config>(configJson) ?? new Config { Projects = new List<Project>() };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading config: {ex.Message}");
                throw;
            }
        }
    }
}
