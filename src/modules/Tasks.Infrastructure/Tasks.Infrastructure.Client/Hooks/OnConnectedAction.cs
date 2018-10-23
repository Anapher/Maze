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
        private readonly ITaskSessionManager _sessionManager;

        public OnConnectedAction(IOrcusRestClient restClient, ClientTaskManager taskManager, ITaskSessionManager sessionManager)
        {
            _restClient = restClient;
            _taskManager = taskManager;
            _sessionManager = sessionManager;
        }

        public async Task Execute()
        {
            var tasks = await TasksResource.GetSyncInfo(_restClient);
            await _taskManager.Synchronize(tasks, _restClient);

            _sessionManager.FetchOutstandingTaskExecutions()
        }
    }
}