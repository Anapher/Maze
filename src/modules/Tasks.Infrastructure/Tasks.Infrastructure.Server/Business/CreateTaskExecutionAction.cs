using System.Threading.Tasks;
using AutoMapper;
using CodeElements.BizRunner;
using CodeElements.BizRunner.Generic;
using Tasks.Infrastructure.Core.Dtos;
using Tasks.Infrastructure.Server.BusinessDataAccess;
using Tasks.Infrastructure.Management.Data;

namespace Tasks.Infrastructure.Server.Business
{
    public interface ICreateTaskExecutionAction : IGenericActionAsync<TaskExecutionDto, TaskExecution>
    {
    }

    public class CreateTaskExecutionAction : BusinessActionErrors, ICreateTaskExecutionAction
    {
        private readonly IExecutionsDbAccess _dbAccess;

        public CreateTaskExecutionAction(IExecutionsDbAccess dbAccess)
        {
            _dbAccess = dbAccess;
        }

        public async Task<TaskExecution> BizActionAsync(TaskExecutionDto inputData)
        {
            if (ValidateModelFailed(inputData))
                return default;

            var taskExecution = Mapper.Map<TaskExecution>(inputData);
            await _dbAccess.CreateAsync(taskExecution);

            return taskExecution;
        }
    }
}