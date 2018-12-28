using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Maze.Client.Library.Clients;
using Maze.Client.Library.Clients.Helpers;
using Maze.Server.Connection.Utilities;
using Tasks.Infrastructure.Core;
using Tasks.Infrastructure.Core.Dtos;

namespace Tasks.Infrastructure.Client.Rest.V1
{
    public class TasksResource : ModuleResource<TasksResource>
    {
        public TasksResource() : base("Tasks.Infrastructure", "tasks")
        {
        }

        public static Task<List<TaskSyncDto>> GetSyncInfo(IRestClient restClient) =>
            CreateRequest(HttpVerb.Get, "sync").Execute(restClient).Return<List<TaskSyncDto>>();

        public static Task UpdateMachineStatus(IList<ClientTaskDto> tasks, IRestClient restClient) =>
            CreateRequest(HttpVerb.Post, "status", tasks).Execute(restClient);

        public static async Task<MazeTask> FetchTaskAsync(Guid taskId, ITaskComponentResolver taskComponentResolver,
            IXmlSerializerCache xmlSerializerCache, IRestClient restClient)
        {
            using (var response = await CreateRequest(HttpVerb.Get, taskId.ToString("N")).Execute(restClient))
            using (var stream = await response.Content.ReadAsStreamAsync())
            {
                var taskReader = new MazeTaskReader(stream, taskComponentResolver, xmlSerializerCache);
                return taskReader.ReadTask();
            }
        }
    }
}