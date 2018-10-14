using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Orcus.Server.Data.EfClasses.Tasks;
using Orcus.Server.Data.EfCode;
using Orcus.Server.Library.Tasks;
using Orcus.Utilities;

namespace Orcus.Server.Service.Tasks
{
    public class TaskSessionTriggerService : TaskSessionTrigger
    {
        private readonly OrcusTaskService _taskService;

        public TaskSessionTriggerService(OrcusTaskService taskService, TaskSession taskSession)
        {
            _taskService = taskService;
            Info = taskSession;
        }

        public override TaskSession Info { get; }

        public override async Task InvokeAll()
        {
            using (var serviceScope = _taskService.Services.CreateScope())
            {
                var dbContext = serviceScope.ServiceProvider.GetRequiredService<AppDbContext>();
                var clients = await dbContext.Clients.Select(x => x.ClientId).ToListAsync();

                //the filter is applied on execute so we just feed it with everything we have
                await _taskService.Execute(clients.Select(x => new TargetId(x)).Concat(TargetId.ServerId.Yield()));
            }
        }

        public override Task InvokeClient(int clientId)
        {
            return _taskService.Execute(new TargetId(clientId).Yield());
        }

        public override Task InvokeServer()
        {
            return _taskService.Execute(TargetId.ServerId.Yield());
        }
    }
}