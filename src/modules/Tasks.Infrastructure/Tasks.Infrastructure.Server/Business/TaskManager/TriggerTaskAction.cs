using System;
using System.Linq;
using System.Threading.Tasks;
using CodeElements.BizRunner;
using CodeElements.BizRunner.Generic;
using Orcus.Server.Library.Services;
using Tasks.Infrastructure.Server.Core;
using Tasks.Infrastructure.Server.Rest;

namespace Tasks.Infrastructure.Server.Business.TaskManager
{
    public interface ITriggerTaskAction : IGenericActionInOnlyAsync<Guid>
    {
    }

    public class TriggerTaskAction : BusinessActionErrors, ITriggerTaskAction
    {
        private readonly ActiveTasksManager _activeTasksManager;
        private readonly IConnectionManager _connectionManager;
        private readonly IOrcusTaskManagerManagement _management;

        public TriggerTaskAction(IOrcusTaskManagerManagement management, IConnectionManager connectionManager, ActiveTasksManager activeTasksManager)
        {
            _management = management;
            _connectionManager = connectionManager;
            _activeTasksManager = activeTasksManager;
        }

        public async Task BizActionAsync(Guid inputData)
        {
            await _management.TriggerNow(inputData);

            foreach (var status in _activeTasksManager.ActiveCommands.Where(x => !x.Key.IsServer))
                if (status.Value.ActiveTasks.Any(x => x.TaskId == inputData))
                    if (_connectionManager.ClientConnections.TryGetValue(status.Key.ClientId, out var connection))
                        await TasksResource.TriggerTask(inputData, connection);
        }
    }
}