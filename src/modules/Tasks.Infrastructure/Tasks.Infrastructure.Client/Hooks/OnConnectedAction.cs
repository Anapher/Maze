using System.Threading.Tasks;
using Maze.Client.Library.Interfaces;
using Maze.Client.Library.Services;
using Tasks.Infrastructure.Client.Rest.V1;

namespace Tasks.Infrastructure.Client.Hooks
{
    public class OnConnectedAction : IConnectedAction
    {
        private readonly IMazeRestClient _restClient;
        private readonly IClientTaskManager _taskManager;

        public OnConnectedAction(ICoreConnector coreConnector, IClientTaskManager taskManager)
        {
            _restClient = coreConnector?.CurrentConnection?.RestClient;
            _taskManager = taskManager;
        }

        public async Task Execute()
        {
            await _taskManager.Initialize();

            if (_restClient == null)
                return;

            var tasks = await TasksResource.GetSyncInfo(_restClient);
            await _taskManager.Synchronize(tasks, _restClient);
        }
    }
}