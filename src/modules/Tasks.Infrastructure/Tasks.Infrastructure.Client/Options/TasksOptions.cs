namespace Tasks.Infrastructure.Client.Options
{
    public class TasksOptions
    {
        public string Directory { get; set; } = "%appdata%\\Orcus\\tasks";
        public string SessionsDirectory { get; set; } = "%appdata%\\Orcus\\tasks\\sessions";
        public string FileExtension { get; set; } = "orcTask";
    }
}