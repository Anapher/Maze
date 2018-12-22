using System.Threading.Tasks;
using AutoMapper;
using CodeElements.BizRunner;
using CodeElements.BizRunner.Generic;
using Tasks.Infrastructure.Core;
using Tasks.Infrastructure.Core.Dtos;
using Tasks.Infrastructure.Server.BusinessDataAccess;
using Tasks.Infrastructure.Management.Data;

namespace Tasks.Infrastructure.Server.Business
{
    public interface ICreateTaskSessionAction : IGenericActionAsync<TaskSessionDto, TaskSession>
    {
    }

    public class CreateTaskSessionAction : BusinessActionErrors, ICreateTaskSessionAction
    {
        private readonly ISessionsDbAccess _dbAccess;
        private readonly ITaskReferenceDbAccess _tasksDbAccess;

        public CreateTaskSessionAction(ISessionsDbAccess dbAccess, ITaskReferenceDbAccess tasksDbAccess)
        {
            _dbAccess = dbAccess;
            _tasksDbAccess = tasksDbAccess;
        }

        public async Task<TaskSession> BizActionAsync(TaskSessionDto inputData)
        {
            if (ValidateModelFailed(inputData))
                return default;

            var taskSession = await _dbAccess.FindAsync(inputData.TaskSessionId, inputData.TaskReferenceId);
            if (taskSession == null)
            {
                var taskReference = await _tasksDbAccess.FindAsync(inputData.TaskReferenceId);
                if (taskReference == null)
                    return ReturnError<TaskSession>(TaskErrors.TaskNotFound);

                taskSession = Mapper.Map<TaskSession>(inputData);
                await _dbAccess.CreateAsync(taskSession);
            }

            return taskSession;
        }
    }
}