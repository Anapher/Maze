using System;
using System.Threading.Tasks;
using CodeElements.BizRunner;
using CodeElements.BizRunner.Generic;
using Orcus.Server.Connection;
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
                return ReturnError<TaskTransmission>(BusinessErrors.FieldNullOrEmpty("TaskReferenceId"));

            if (inputData.CreatedOn == default)
                return ReturnError<TaskTransmission>(BusinessErrors.FieldNullOrEmpty("CreatedOn"));

            await _dbAccess.CreateAsync(inputData);
            return inputData;
        }
    }
}