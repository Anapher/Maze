using System;
using System.Threading.Tasks;
using CodeElements.BizRunner;
using CodeElements.BizRunner.Generic;
using Tasks.Infrastructure.Management;
using Tasks.Infrastructure.Server.BusinessDataAccess;
using Tasks.Infrastructure.Server.Core;

namespace Tasks.Infrastructure.Server.Business.TaskManager
{
    public interface IDeleteTaskAction : IGenericActionInOnlyAsync<Guid>
    {
    }

    public class DeleteTaskAction : BusinessActionErrors, IDeleteTaskAction
    {
        private readonly IMazeTaskManagerManagement _management;
        private readonly ITaskReferenceDbAccess _dbAccess;
        private readonly ITaskDirectory _taskDirectory;

        public DeleteTaskAction(IMazeTaskManagerManagement management, ITaskReferenceDbAccess dbAccess, ITaskDirectory taskDirectory)
        {
            _management = management;
            _dbAccess = dbAccess;
            _taskDirectory = taskDirectory;
        }

        public async Task BizActionAsync(Guid inputData)
        {
            await _management.CancelTask(inputData);

            await _taskDirectory.RemoveTask(inputData);
            await _dbAccess.DeleteAsync(inputData);
        }
    }
}