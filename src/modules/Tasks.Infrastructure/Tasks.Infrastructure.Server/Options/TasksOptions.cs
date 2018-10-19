namespace Tasks.Infrastructure.Server.Options
{
    public class TasksOptions
    {
        public string Directory { get; set; }
        public string TaskFileExtension { get; set; } = "orcTask";
    }
}