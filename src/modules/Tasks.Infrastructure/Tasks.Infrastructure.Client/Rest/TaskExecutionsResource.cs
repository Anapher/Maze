using System.Net.Http;
using System.Threading.Tasks;
using Orcus.Client.Library.Clients;
using Orcus.Client.Library.Clients.Helpers;
using Tasks.Infrastructure.Core.Dtos;
using Tasks.Infrastructure.Management.Data;

namespace Tasks.Infrastructure.Client.Rest
{
    public class TaskExecutionsResource : VersionedResource<TaskExecutionsResource>
    {
        public TaskExecutionsResource() : base("executions")
        {
        }

        public static HttpRequestMessage CreateExecutionRequest(TaskExecution taskExecution) =>
            CreateRequest(HttpVerb.Post, null, taskExecution).Build();

        public static HttpRequestMessage CreateCommandResultRequest(CommandResult commandResult) =>
            CreateRequest(HttpVerb.Post, "results", commandResult).Build();

        public static Task ReportProgress(CommandProcessDto commandProcessDto, IRestClient restClient) =>
            CreateRequest(HttpVerb.Post, "process", commandProcessDto).Execute(restClient);
    }
}