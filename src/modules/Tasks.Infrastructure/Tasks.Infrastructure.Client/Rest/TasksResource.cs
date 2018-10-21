using System.Collections.Generic;
using System.Threading.Tasks;
using Orcus.Client.Library.Clients;
using Orcus.Client.Library.Clients.Helpers;
using Tasks.Infrastructure.Core.Dtos;

namespace Tasks.Infrastructure.Client.Rest
{
    public class TasksResource : VersionedResource<TasksResource>
    {
        public TasksResource() : base("tasks")
        {
        }

        public static Task<List<TaskSyncDto>> GetSyncInfo(IRestClient restClient) =>
            CreateRequest(HttpVerb.Get, "sync").Execute(restClient).Return<List<TaskSyncDto>>();
    }
}