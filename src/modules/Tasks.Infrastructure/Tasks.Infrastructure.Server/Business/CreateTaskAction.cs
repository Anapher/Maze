using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using CodeElements.BizRunner;
using CodeElements.BizRunner.Generic;
using Tasks.Infrastructure.Core;
using Tasks.Infrastructure.Core.Data;

namespace Tasks.Infrastructure.Server.Business
{
    public interface ICreateTaskAction : IGenericActionWriteDbAsync<OrcusTask, TaskReference>
    {
    }

    public class CreateTaskAction : BusinessActionErrors, ICreateTaskAction
    {
        public Task<TaskReference> BizActionAsync(OrcusTask inputData)
        {
            AddValidationResults(inputData.Validate(new ValidationContext(inputData)));
            if (HasErrors)
                return null;

            return null;
        }
    }
}