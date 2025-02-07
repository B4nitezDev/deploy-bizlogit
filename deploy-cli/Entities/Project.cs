namespace deploy_cli.Entities
{
    public class Project
    {
        public string Name { get; set; } = string.Empty;
        public string Path { get; set; } = string.Empty;
        public string DestPath { get; set; } = string.Empty;
        public int Version { get; set; }
        public string PowerShellScript { get; set; } = string.Empty;
    }
}
