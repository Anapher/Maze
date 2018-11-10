using System.Net.Http;
using Orcus.Client.Library.Clients.Helpers;
using Tasks.Infrastructure.Core.Dtos;
using Tasks.Infrastructure.Management.Data;

namespace Tasks.Infrastructure.Client.Rest
{
    public class TaskSessionsResource : VersionedResource<TaskSessionsResource>
    {
        public TaskSessionsResource() : base("sessions")
        {
        }

        public static HttpRequestMessage CreateSessionRequest(TaskSession taskSession) =>
            CreateRequest(HttpVerb.Post, "sessions", taskSession).Build();
    }
}