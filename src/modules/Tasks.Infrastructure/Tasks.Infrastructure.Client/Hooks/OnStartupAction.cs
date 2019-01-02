using System.Threading.Tasks;
using Maze.Client.Library.Interfaces;

namespace Tasks.Infrastructure.Client.Hooks
{
    public class OnStartupAction : IStartupAction
    {
        private readonly IClientTaskManager _taskManager;

        public OnStartupAction(IClientTaskManager taskManager)
        {
            _taskManager = taskManager;
        }

        public async Task Execute()
        {
            await _taskManager.Initialize();
        }
    }
}