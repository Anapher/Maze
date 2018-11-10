using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Orcus.Client.Library.Clients;
using Orcus.Client.Library.Clients.Helpers;
using Orcus.Server.Connection.Utilities;
using Tasks.Infrastructure.Core;
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

        public static async Task<OrcusTask> FetchTaskAsync(Guid taskId, ITaskComponentResolver taskComponentResolver,
            IXmlSerializerCache xmlSerializerCache, IRestClient restClient)
        {
            using (var response = await CreateRequest(HttpVerb.Get, taskId.ToString("N")).Execute(restClient))
            using (var stream = await response.Content.ReadAsStreamAsync())
            {
                var taskReader = new OrcusTaskReader(stream, taskComponentResolver, xmlSerializerCache);
                return taskReader.ReadTask();
            }
        }
    }
}