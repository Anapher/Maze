using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using CodeElements.BizRunner;
using CodeElements.BizRunner.Generic;
using Tasks.Infrastructure.Core;
using Tasks.Infrastructure.Server.BusinessDataAccess;
using Tasks.Infrastructure.Management.Data;

namespace Tasks.Infrastructure.Server.Business
{
    public interface ICreateTaskAction : IGenericActionAsync<OrcusTask, TaskReference>
    {
    }

    public class CreateTaskAction : BusinessActionErrors, ICreateTaskAction
    {
        private readonly ITaskReferenceDbAccess _dbAccess;

        public CreateTaskAction(ITaskReferenceDbAccess dbAccess)
        {
            _dbAccess = dbAccess;
        }

        public async Task<TaskReference> BizActionAsync(OrcusTask inputData)
        {
            AddValidationResults(inputData.Validate(new ValidationContext(inputData)));
            if (HasErrors)
                return null;

            var taskReference = await _dbAccess.FindAsync(inputData.Id);
            if (taskReference == null)
            {
                taskReference = new TaskReference {TaskId = inputData.Id, IsCompleted = false, AddedOn = DateTimeOffset.UtcNow};
                await _dbAccess.CreateAsync(taskReference);
            }

            return taskReference;
        }
    }
}