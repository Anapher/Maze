using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Orcus.Server.BusinessLogic.Tasks;
using Orcus.Server.Data.EfClasses.Tasks;
using Orcus.Server.Data.EfCode;
using Orcus.Server.Library.Tasks;

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

        public override async Task<TaskSessionTrigger> GetSession(string name)
        {
            TaskSession taskSession;
            using (var scope = _taskService.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                var action = _taskService.Services.GetRequiredService<IGetOrCreateTaskSessionAction>();
                taskSession = await action.BizActionAsync((_taskService.OrcusTask.Id, name, _sourceTrigger));
                if (action.HasErrors)
                    throw new InvalidOperationException(action.Errors.First().ErrorMessage);

                await context.SaveChangesAsync();
            }

            return new TaskSessionTriggerService(_taskService, taskSession);
        }

        public override Task<bool> IsClientIncluded(int clientId)
        {
            return _aggregatedClientFilter.IsClientIncluded(clientId);
        }

        public override bool IsServerIncluded()
        {
            return _taskService.OrcusTask.Audience.IncludesServer;
        }
    }
}