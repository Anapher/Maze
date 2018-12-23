using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Tasks.Infrastructure.Core.Dtos;
using Tasks.Infrastructure.Management.Data;

namespace Tasks.Infrastructure.Server.Core.Storage
{
    public class MemoryTaskResultStorage : ITaskResultStorage
    {
        private readonly object _commandResultsLock = new object();
        private readonly object _executionsLock = new object();
        private readonly object _sessionsLock = new object();

        public List<TaskSessionDto> Sessions { get; } = new List<TaskSessionDto>();
        public List<TaskExecutionDto> Executions { get; } = new List<TaskExecutionDto>();
        public List<CommandResultDto> CommandResults { get; } = new List<CommandResultDto>();

        Task<TaskSession> ITaskResultStorage.CreateTaskSession(TaskSessionDto taskSession)
        {
            lock (_sessionsLock)
            {
                Sessions.Add(taskSession);
            }

            return Task.FromResult(Mapper.Map<TaskSession>(taskSession));
        }

        Task<bool> ITaskResultStorage.CreateTaskExecution(TaskExecutionDto taskExecution)
        {
            lock (_executionsLock)
            {
                Executions.Add(taskExecution);
            }

            return Task.FromResult(true);
        }

        Task ITaskResultStorage.CreateCommandResult(CommandResultDto commandResult)
        {
            lock (_commandResultsLock)
            {
                CommandResults.Add(commandResult);
            }

            return Task.CompletedTask;
        }
    }
}