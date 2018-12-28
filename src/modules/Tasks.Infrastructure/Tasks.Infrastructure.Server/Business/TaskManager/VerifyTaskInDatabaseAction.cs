using System;
using System.Threading.Tasks;
using CodeElements.BizRunner;
using CodeElements.BizRunner.Generic;
using Tasks.Infrastructure.Core;
using Tasks.Infrastructure.Management.Data;
using Tasks.Infrastructure.Server.BusinessDataAccess;

namespace Tasks.Infrastructure.Server.Business.TaskManager
{
    public interface IVerifyTaskInDatabaseAction : IGenericActionAsync<MazeTask, TaskReference>
    {
    }

    public class VerifyTaskInDatabaseAction : BusinessActionErrors, IVerifyTaskInDatabaseAction
    {
        private readonly ITaskReferenceDbAccess _dbAccess;

        public VerifyTaskInDatabaseAction(ITaskReferenceDbAccess dbAccess)
        {
            _dbAccess = dbAccess;
        }

        public async Task<TaskReference> BizActionAsync(MazeTask inputData)
        {
            if (ValidateModelFailed(inputData))
                return null;

            var taskReference = await _dbAccess.FindAsync(inputData.Id);
            if (taskReference == null)
            {
                taskReference = new TaskReference {TaskId = inputData.Id, IsCompleted = false, IsEnabled = true, AddedOn = DateTimeOffset.UtcNow};
                await _dbAccess.CreateAsync(taskReference);
            }

            return taskReference;
        }
    }
}