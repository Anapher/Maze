using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using CodeElements.BizRunner;
using CodeElements.BizRunner.Generic;
using Tasks.Infrastructure.Management.Data;
using Tasks.Infrastructure.Server.BusinessDataAccess;

namespace Tasks.Infrastructure.Server.Business
{
    public interface ICreateTaskTransmissionAction : IGenericActionAsync<TaskTransmission, TaskTransmission>
    {
    }

    public class CreateTaskTransmissionAction : BusinessActionErrors, ICreateTaskTransmissionAction
    {
        private readonly ITaskTransmissionsDbAccess _dbAccess;

        public CreateTaskTransmissionAction(ITaskTransmissionsDbAccess dbAccess)
        {
            _dbAccess = dbAccess;
        }

        public async Task<TaskTransmission> BizActionAsync(TaskTransmission inputData)
        {
            if (inputData.TaskReferenceId == Guid.Empty)
                return ReturnError<TaskTransmission>(new ValidationResult("The TaskReferenceId must not be empty"));

            if (inputData.CreatedOn == default)
                return ReturnError<TaskTransmission>(new ValidationResult("The CreatedOn must not be the default value"));

            await _dbAccess.CreateAsync(inputData);
            return inputData;
        }
    }
}