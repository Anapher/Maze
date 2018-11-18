using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Orcus.Server.Connection.Utilities;
using Orcus.Server.Library.Clients;
using Orcus.Server.Library.Clients.Helpers;
using Tasks.Infrastructure.Core;

namespace Tasks.Infrastructure.Server.Rest
{
    public class TasksResource : Resource<TasksResource>
    {
        public override string ResourceUri { get; } = "Tasks.Infrastructure/v1/tasks";

        public static async Task CreateOrUpdateTask(OrcusTask orcusTask, ITaskComponentResolver componentResolver, IXmlSerializerCache xmlCache,
            IRestClient restClient)
        {
            using (var taskMemoryStream = new MemoryStream())
            {
                var stream = new StreamContent(taskMemoryStream);
                stream.Headers.ContentEncoding.Add("xml");
                var taskWriter = new OrcusTaskWriter(taskMemoryStream, componentResolver, xmlCache);
                taskWriter.Write(orcusTask, TaskDetails.Client);

                await CreateRequest(HttpVerb.Post, null, stream).Execute(restClient);
            }
        }
    }
}