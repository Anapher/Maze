using System.Threading.Tasks;
using CodeElements.BizRunner;
using CodeElements.BizRunner.Generic;
using Tasks.Infrastructure.Core;
using Tasks.Infrastructure.Management;
using Tasks.Infrastructure.Server.BusinessDataAccess;
using Tasks.Infrastructure.Server.Core;

namespace Tasks.Infrastructure.Server.Business.TaskManager
{
    public interface IUpdateTaskAction : IGenericActionInOnlyAsync<MazeTask> { }

    public class UpdateTaskAction : BusinessActionErrors, IUpdateTaskAction
    {
        private readonly IMazeTaskManagerManagement _management;
        private readonly ITaskReferenceDbAccess _dbAccess;
        private readonly ITaskDirectory _taskDirectory;

        public UpdateTaskAction(IMazeTaskManagerManagement management, ITaskReferenceDbAccess dbAccess, ITaskDirectory taskDirectory)
        {
            _management = management;
            _dbAccess = dbAccess;
            _taskDirectory = taskDirectory;
        }

        public async Task BizActionAsync(MazeTask inputData)
        {
            if (ValidateModelFailed(inputData))
                return;

            var taskReference = await _dbAccess.FindAsync(inputData.Id);
            if (taskReference == null)
            {
                AddValidationResult(TaskErrors.TaskNotFound);
                return;
            }

            await _taskDirectory.WriteTask(inputData);

            if (taskReference.IsEnabled)
            {
                var hash = _taskDirectory.ComputeTaskHash(inputData);

                await _management.CancelTask(inputData.Id);
                await _management.InitializeTask(inputData, hash, transmit: true, executeLocally: true);
            }
        }
    }
}
