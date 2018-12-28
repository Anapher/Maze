namespace Tasks.Infrastructure.Client.Options
{
    public class TasksOptions
    {
        public string Directory { get; set; } = "%appdata%\\Maze\\tasks";
        public string SessionsDirectory { get; set; } = "%appdata%\\Maze\\tasks\\sessions";
        public string FileExtension { get; set; } = "orcTask";
    }
}