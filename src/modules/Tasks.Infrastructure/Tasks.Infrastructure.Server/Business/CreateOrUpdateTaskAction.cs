using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using CodeElements.BizRunner;
using CodeElements.BizRunner.Generic;
using Microsoft.EntityFrameworkCore;
using Tasks.Infrastructure.Core;
using Tasks.Infrastructure.Core.Data;
using Tasks.Infrastructure.Server.Data;

namespace Tasks.Infrastructure.Server.Business
{
    public interface ICreateOrUpdateTaskAction : IGenericActionWriteDbAsync<(OrcusTask, string filename), TaskReference>
    {
    }

    public class CreateOrUpdateTaskAction : BusinessActionErrors, ICreateOrUpdateTaskAction
    {
        private readonly TasksDbContext _dbContext;

        public CreateOrUpdateTaskAction(TasksDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<TaskReference> BizActionAsync((OrcusTask, string filename) inputData)
        {
            var (task, filename) = inputData;

            AddValidationResults(task.Validate(new ValidationContext(inputData)));
            if (HasErrors)
                return null;

            var taskReference = await _dbContext.Tasks.FirstOrDefaultAsync(x => x.TaskId == task.Id);
            if (taskReference == null)
            {
                taskReference = new TaskReference{TaskId = task.Id};
                _dbContext.Add(taskReference);
            }

            taskReference.Filename = filename;
            taskReference.IsFinished = false;

            return taskReference;
        }
    }
}