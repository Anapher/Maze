using System.Threading.Tasks;
using Orcus.Client.Library.Interfaces;

namespace Tasks.Infrastructure.Client.Hooks
{
    public class StartupAction : IStartupAction
    {
        private readonly ClientTaskManager _clientTaskManager;

        public StartupAction(ClientTaskManager clientTaskManager, ITaskSessionManager taskSessionManager)
        {
            _clientTaskManager = clientTaskManager;
        }

        public async Task Execute()
        {
            await _clientTaskManager.Initialize();
        }
    }
}