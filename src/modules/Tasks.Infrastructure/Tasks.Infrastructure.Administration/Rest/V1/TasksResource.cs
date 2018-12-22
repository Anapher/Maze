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
    public class TasksResource : ModuleResource<TasksResource>
    {
        public TasksResource() : base(PrismModule.ModuleName, "tasks")
        {
        }

        public static async Task Create(OrcusTask orcusTask, ITaskComponentResolver componentResolver, IXmlSerializerCache xmlCache,
            IRestClient restClient)
        {
            using (var taskMemoryStream = new MemoryStream())
            {
                var taskWriter = new OrcusTaskWriter(taskMemoryStream, componentResolver, xmlCache);
                taskWriter.Write(orcusTask, TaskDetails.Server);

                taskMemoryStream.Position = 0;

                var stream = new StreamContent(taskMemoryStream);
                stream.Headers.ContentEncoding.Add("xml");

                await CreateRequest(HttpVerb.Post, null, stream).Execute(restClient);
            }
        }

        public static async Task Update(OrcusTask orcusTask, ITaskComponentResolver componentResolver, IXmlSerializerCache xmlCache,
            IRestClient restClient)
        {
            using (var taskMemoryStream = new MemoryStream())
            {
                var taskWriter = new OrcusTaskWriter(taskMemoryStream, componentResolver, xmlCache);
                taskWriter.Write(orcusTask, TaskDetails.Server);

                taskMemoryStream.Position = 0;

                var stream = new StreamContent(taskMemoryStream);
                stream.Headers.ContentEncoding.Add("xml");

                await CreateRequest(HttpVerb.Put, orcusTask.Id, stream).Execute(restClient);
            }
        }

        public static Task<List<TaskInfoDto>> GetTasks(IRestClient restClient) => CreateRequest().Execute(restClient).Return<List<TaskInfoDto>>();

        public static Task<TaskSessionsInfo> GetTaskSessions(Guid taskId, IRestClient restClient) =>
            CreateRequest(HttpVerb.Get, taskId + "/sessions").Execute(restClient).Return<TaskSessionsInfo>();

        public static Task RemoveTask(Guid taskId, IRestClient restClient) =>
            CreateRequest(HttpVerb.Delete, taskId).Execute(restClient);

        public static Task EnableTask(Guid taskId, IRestClient restClient) =>
            CreateRequest(HttpVerb.Post, taskId + "/enable").Execute(restClient);

        public static Task DisableTask(Guid taskId, IRestClient restClient) =>
            CreateRequest(HttpVerb.Post, taskId + "/disable").Execute(restClient);

        public static Task TriggerTask(Guid taskId, IRestClient restClient) =>
            CreateRequest(HttpVerb.Get, taskId + "/trigger").Execute(restClient);

        public static Task<TaskInfoDto> GetTaskInfo(Guid taskId, IRestClient restClient) =>
            CreateRequest(HttpVerb.Get, taskId + "/info").Execute(restClient).Return<TaskInfoDto>();

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