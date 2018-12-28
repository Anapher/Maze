using System.Net.Http;
using Maze.Client.Library.Clients;
using Maze.Client.Library.Clients.Helpers;
using Tasks.Infrastructure.Management.Data;

namespace Tasks.Infrastructure.Client.Rest.V1
{
    public class TaskSessionsResource : ModuleResource<TaskSessionsResource>
    {
        public TaskSessionsResource() : base("Tasks.Infrastructure", "sessions")
        {
        }

        public static HttpRequestMessage CreateSessionRequest(TaskSession taskSession) =>
            CreateRequest(HttpVerb.Post, null, taskSession).Build();
    }
}