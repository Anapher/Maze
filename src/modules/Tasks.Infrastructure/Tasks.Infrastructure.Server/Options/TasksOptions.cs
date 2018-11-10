namespace Tasks.Infrastructure.Server.Options
{
    public class TasksOptions
    {
        public string ConnectionString { get; set; } = "Data Source=tasks/tasks.sqlite";
        public string Directory { get; set; } = "tasks";
        public string FileExtension { get; set; } = "orcTask";
    }
}