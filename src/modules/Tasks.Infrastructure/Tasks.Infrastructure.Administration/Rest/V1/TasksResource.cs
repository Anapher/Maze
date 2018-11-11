using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Orcus.Administration.Library.Clients;
using Orcus.Administration.Library.Clients.Helpers;
using Orcus.Server.Connection.Utilities;
using Tasks.Infrastructure.Administration.Hooks;
using Tasks.Infrastructure.Core;
using Tasks.Infrastructure.Core.Dtos;

namespace Tasks.Infrastructure.Administration.Rest.V1
{
    public class TasksResource : ResourceBase<TasksResource>
    {
        public TasksResource() : base($"{PrismModule.ModuleName}/v1/tasks")
        {
        }

        public static async Task Create(OrcusTask orcusTask, ITaskComponentResolver componentResolver, IXmlSerializerCache xmlCache,
            IRestClient restClient)
        {
            using (var taskMemoryStream = new MemoryStream())
            {
                var stream = new StreamContent(taskMemoryStream);
                stream.Headers.ContentEncoding.Add("xml");
                var taskWriter = new OrcusTaskWriter(taskMemoryStream, componentResolver, xmlCache);
                taskWriter.Write(orcusTask, TaskDetails.Server);

                await CreateRequest(HttpVerb.Post, null, stream).Execute(restClient);
            }
        }

        public static Task<List<TaskInfoDto>> GetTasks(IRestClient restClient) => CreateRequest().Execute(restClient).Return<List<TaskInfoDto>>();

        public static Task<TaskSessionsInfo> GetTaskSessions(Guid taskId, IRestClient restClient) =>
            CreateRequest(HttpVerb.Get, taskId + "/sessions").Execute(restClient).Return<TaskSessionsInfo>();
    }
}