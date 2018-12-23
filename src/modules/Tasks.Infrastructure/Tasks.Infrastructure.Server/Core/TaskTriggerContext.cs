using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Tasks.Infrastructure.Core.Dtos;
using Tasks.Infrastructure.Server.Core.Storage;
using Tasks.Infrastructure.Server.Filter;
using Tasks.Infrastructure.Server.Library;

namespace Tasks.Infrastructure.Server.Core
{
    public class TaskTriggerContext : TriggerContext
    {
        private readonly OrcusTaskService _taskService;
        private readonly string _sourceTrigger;
        private readonly AggregatedClientFilter _aggregatedClientFilter;
        private readonly ITaskResultStorage _taskResultStorage;

        public TaskTriggerContext(OrcusTaskService taskService, string sourceTrigger, AggregatedClientFilter aggregatedClientFilter,
            ITaskResultStorage taskResultStorage)
        {
            _taskService = taskService;
            _sourceTrigger = sourceTrigger;
            _aggregatedClientFilter = aggregatedClientFilter;
            _taskResultStorage = taskResultStorage;
        }

        public override Task<TaskSessionTrigger> CreateSession(SessionKey sessionKey)
        {
            return CreateSession(sessionKey, _sourceTrigger);
        }

        public override async Task<TaskSessionTrigger> CreateSession(SessionKey sessionKey, string description)
        {
            var taskSession = await _taskResultStorage.CreateTaskSession(new TaskSessionDto
            {
                TaskSessionId = sessionKey.Hash,
                Description = description,
                TaskReferenceId = _taskService.OrcusTask.Id,
                CreatedOn = DateTimeOffset.UtcNow
            });

            return new TaskSessionTriggerService(_taskService, taskSession);
        }

        public override async Task<bool> IsClientIncluded(int clientId)
        {
            using (var scope = _taskService.Services.CreateScope())
            {
                return await _aggregatedClientFilter.IsClientIncluded(clientId, scope.ServiceProvider);
            }
        }

        public override bool IsServerIncluded()
        {
            return _taskService.OrcusTask.Audience.IncludesServer;
        }

        public override void ReportNextTrigger(DateTimeOffset dateTimeOffset)
        {
            _taskService.NextExecution = dateTimeOffset;
        }
    }
}