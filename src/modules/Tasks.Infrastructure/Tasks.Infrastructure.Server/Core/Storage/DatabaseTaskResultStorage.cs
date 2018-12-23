using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Tasks.Infrastructure.Core.Dtos;
using Tasks.Infrastructure.Management.Data;
using Tasks.Infrastructure.Server.Business;

namespace Tasks.Infrastructure.Server.Core.Storage
{
    public class DatabaseTaskResultStorage : ITaskResultStorage
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<DatabaseTaskResultStorage> _logger;

        public DatabaseTaskResultStorage(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _logger = serviceProvider.GetRequiredService<ILogger<DatabaseTaskResultStorage>>();
        }

        public async Task<TaskSession> CreateTaskSession(TaskSessionDto taskSession)
        {
            var action = _serviceProvider.GetRequiredService<ICreateTaskSessionAction>();
            var result = await action.BizActionAsync(taskSession);

            if (action.HasErrors)
                throw new InvalidOperationException(action.Errors.First().ErrorMessage);

            return result;
        }

        public async Task<bool> CreateTaskExecution(TaskExecutionDto taskExecution)
        {
            var action = _serviceProvider.GetRequiredService<ICreateTaskExecutionAction>();
            await action.BizActionAsync(taskExecution);

            if (action.HasErrors)
            {
                _logger.LogError("An error occurred when trying to execute {action}: {error}", action.GetType().Name,
                    action.Errors.First().ErrorMessage);
                return false;
            }

            return true;
        }

        public Task CreateCommandResult(CommandResultDto commandResult)
        {
            var action = _serviceProvider.GetRequiredService<ICreateTaskCommandResultAction>();
            return action.BizActionAsync(commandResult);
        }
    }
}
