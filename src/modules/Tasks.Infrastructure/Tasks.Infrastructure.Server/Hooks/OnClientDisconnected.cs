using System.Threading.Tasks;
using Maze.Server.Library.Interfaces;
using Tasks.Infrastructure.Server.Core;
using Tasks.Infrastructure.Server.Library;

namespace Tasks.Infrastructure.Server.Hooks
{
    public class OnClientDisconnected : IClientDisconnectedAction
    {
        private readonly ActiveTasksManager _activeTasksManager;

        public OnClientDisconnected(ActiveTasksManager activeTasksManager)
        {
            _activeTasksManager = activeTasksManager;
        }

        public Task Execute(int context)
        {
            _activeTasksManager.ActiveCommands.TryRemove(new TargetId(context), out _);
            return Task.CompletedTask;
        }
    }
}