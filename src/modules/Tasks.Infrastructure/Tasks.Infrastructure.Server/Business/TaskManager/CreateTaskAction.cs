using System;
using System.Threading.Tasks;
using CodeElements.BizRunner;
using CodeElements.BizRunner.Generic;
using Tasks.Infrastructure.Core;
using Tasks.Infrastructure.Management;
using Tasks.Infrastructure.Management.Data;
using Tasks.Infrastructure.Server.BusinessDataAccess;
using Tasks.Infrastructure.Server.Core;

namespace Tasks.Infrastructure.Server.Business.TaskManager
{
    public interface ICreateTaskAction : IGenericActionAsync<MazeTask, TaskReference>
    {
    }

    public class CreateTaskAction : BusinessActionErrors, ICreateTaskAction
    {
        private readonly ITaskReferenceDbAccess _dbAccess;
        private readonly IMazeTaskManagerManagement _management;
        private readonly ITaskDirectory _taskDirectory;

        public CreateTaskAction(ITaskReferenceDbAccess dbAccess, IMazeTaskManagerManagement management, ITaskDirectory taskDirectory)
        {
            _dbAccess = dbAccess;
            _management = management;
            _taskDirectory = taskDirectory;
        }
        
        public async Task<TaskReference> BizActionAsync(MazeTask inputData)
        {
            if (ValidateModelFailed(inputData))
                return null;

            var taskReference = await _dbAccess.FindAsync(inputData.Id);
            if (taskReference != null)
                return ReturnError<TaskReference>(TaskErrors.TaskAlreadyExists);

            await _taskDirectory.WriteTask(inputData);

            taskReference = new TaskReference {TaskId = inputData.Id, IsCompleted = false, IsEnabled = true, AddedOn = DateTimeOffset.UtcNow};
            await _dbAccess.CreateAsync(taskReference);

            var hash = _taskDirectory.ComputeTaskHash(inputData);
            await _management.InitializeTask(inputData, hash, transmit: true, executeLocally: true);

            return taskReference;
        }
    }
}