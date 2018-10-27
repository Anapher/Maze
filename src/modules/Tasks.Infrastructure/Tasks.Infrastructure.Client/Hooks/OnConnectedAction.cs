using System.Threading.Tasks;
using Orcus.Client.Library.Interfaces;
using Orcus.Client.Library.Services;
using Tasks.Infrastructure.Client.Rest;

namespace Tasks.Infrastructure.Client.Hooks
{
    public class OnConnectedAction : IConnectedAction
    {
        private readonly IOrcusRestClient _restClient;
        private readonly ClientTaskManager _taskManager;

        public OnConnectedAction(IOrcusRestClient restClient, ClientTaskManager taskManager)
        {
            _restClient = restClient;
            _taskManager = taskManager;
        }

        public async Task Execute()
        {
            var tasks = await TasksResource.GetSyncInfo(_restClient);
            await _taskManager.Synchronize(tasks, _restClient);
        }
    }
}