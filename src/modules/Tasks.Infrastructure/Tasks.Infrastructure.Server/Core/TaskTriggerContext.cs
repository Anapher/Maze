using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Tasks.Infrastructure.Core.Dtos;
using Tasks.Infrastructure.Management.Data;
using Tasks.Infrastructure.Server.Business;
using Tasks.Infrastructure.Server.Filter;
using Tasks.Infrastructure.Server.Library;

namespace Tasks.Infrastructure.Server.Core
{
    public class TaskTriggerContext : TriggerContext
    {
        private readonly OrcusTaskService _taskService;
        private readonly string _sourceTrigger;
        private readonly AggregatedClientFilter _aggregatedClientFilter;

        public TaskTriggerContext(OrcusTaskService taskService, string sourceTrigger, AggregatedClientFilter aggregatedClientFilter)
        {
            _taskService = taskService;
            _sourceTrigger = sourceTrigger;
            _aggregatedClientFilter = aggregatedClientFilter;
        }

        public override Task<TaskSessionTrigger> CreateSession(SessionKey sessionKey)
        {
            return CreateSession(sessionKey, _sourceTrigger);
        }

        public override async Task<TaskSessionTrigger> CreateSession(SessionKey sessionKey, string description)
        {
            TaskSession taskSession;
            using (var scope = _taskService.Services.CreateScope())
            {
                var action = scope.ServiceProvider.GetRequiredService<ICreateTaskSessionAction>();
                taskSession = await action.BizActionAsync(new TaskSessionDto
                {
                    TaskSessionId = sessionKey.Hash,
                    Description = description,
                    TaskReferenceId = _taskService.OrcusTask.Id,
                    CreatedOn = DateTimeOffset.UtcNow
                });

                if (action.HasErrors)
                    throw new InvalidOperationException(action.Errors.First().ErrorMessage);
            }

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
            throw new NotImplementedException();
        }
    }
}