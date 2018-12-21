using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using CodeElements.BizRunner;
using CodeElements.BizRunner.Generic;
using Tasks.Infrastructure.Management;
using Tasks.Infrastructure.Server.BusinessDataAccess;
using Tasks.Infrastructure.Server.Core;

namespace Tasks.Infrastructure.Server.Business.TaskManager
{
    public interface IEnableTaskAction : IGenericActionInOnlyAsync<Guid> { }

    public class EnableTaskAction : BusinessActionErrors, IEnableTaskAction
    {
        private readonly IOrcusTaskManagerManagement _management;
        private readonly ITaskReferenceDbAccess _dbAccess;
        private readonly ITaskDirectory _taskDirectory;

        public EnableTaskAction(IOrcusTaskManagerManagement management, ITaskReferenceDbAccess dbAccess, ITaskDirectory taskDirectory)
        {
            _management = management;
            _dbAccess = dbAccess;
            _taskDirectory = taskDirectory;
        }
        
        public async Task BizActionAsync(Guid inputData)
        {
            var tasks = await _taskDirectory.LoadTasks();
            var task = tasks.FirstOrDefault(x => x.Id == inputData);
            if (task == null)
            {
                AddValidationResult(new ValidationResult("The task does not exist."));
                return;
            }

            await _dbAccess.SetTaskIsEnabled(inputData, true);
            var taskReference = await _dbAccess.FindAsync(inputData);
            var hash = _taskDirectory.ComputeTaskHash(task);

            _management.InitializeTask(task, hash, true, !taskReference.IsCompleted);
        }
    }
}