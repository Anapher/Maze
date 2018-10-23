using System.Threading.Tasks;
using Orcus.Client.Library.Interfaces;

namespace Tasks.Infrastructure.Client.Hooks
{
    public class StartupAction : IStartupAction
    {
        private readonly ClientTaskManager _clientTaskManager;
        private readonly ITaskSessionManager _taskSessionManager;
        private readonly TaskExecutionTransmitter _executionTransmitter;

        public StartupAction(ClientTaskManager clientTaskManager, ITaskSessionManager taskSessionManager,
            TaskExecutionTransmitter executionTransmitter)
        {
            _clientTaskManager = clientTaskManager;
            _taskSessionManager = taskSessionManager;
            _executionTransmitter = executionTransmitter;
        }

        public async Task Execute()
        {
            await _clientTaskManager.Initialize();

            var taskExecutions = await _taskSessionManager.FetchOutstandingTaskExecutions();
            _executionTransmitter.Initialize(taskExecutions);
        }
    }
}