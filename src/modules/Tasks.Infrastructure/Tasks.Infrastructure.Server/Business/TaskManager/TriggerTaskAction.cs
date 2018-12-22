using System;
using System.Linq;
using System.Threading.Tasks;
using CodeElements.BizRunner;
using CodeElements.BizRunner.Generic;
using Orcus.Server.Library.Services;
using Tasks.Infrastructure.Core;
using Tasks.Infrastructure.Management;
using Tasks.Infrastructure.Server.Core;
using Tasks.Infrastructure.Server.Rest.V1;

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
        private readonly ITaskDirectory _taskDirectory;

        public TriggerTaskAction(IOrcusTaskManagerManagement management, ITaskDirectory taskDirectory, IConnectionManager connectionManager, ActiveTasksManager activeTasksManager)
        {
            _management = management;
            _taskDirectory = taskDirectory;
            _connectionManager = connectionManager;
            _activeTasksManager = activeTasksManager;
        }

        public async Task BizActionAsync(Guid inputData)
        {
            var task = (await _taskDirectory.LoadTasks()).FirstOrDefault(x => x.Id == inputData);
            if (task == null)
            {
                AddValidationResult(TaskErrors.TaskNotFound);
                return;
            }

            await _management.TriggerNow(task);

            foreach (var status in _activeTasksManager.ActiveCommands.Where(x => !x.Key.IsServer))
                if (status.Value.Tasks.Any(x => x.TaskId == inputData))
                    if (_connectionManager.ClientConnections.TryGetValue(status.Key.ClientId, out var connection))
                        await TasksResource.TriggerTask(inputData, connection);
        }
    }
}