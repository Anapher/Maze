using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Orcus.Server.BusinessLogic.Tasks;
using Orcus.Server.Data.EfClasses.Tasks;
using Orcus.Server.Data.EfCode;
using Orcus.Server.Library.Tasks;
using Orcus.Server.Service.Tasks.Filter;

namespace Orcus.Server.Service.Tasks
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
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                var action = _taskService.Services.GetRequiredService<IGetOrCreateTaskSessionAction>();
                taskSession = await action.BizActionAsync((_taskService.OrcusTask.Id, sessionKey.Hash, description));
                if (action.HasErrors)
                    throw new InvalidOperationException(action.Errors.First().ErrorMessage);

                await context.SaveChangesAsync();
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

        public override void TellNextTrigger(DateTimeOffset dateTimeOffset)
        {
            throw new NotImplementedException();
        }
    }
}