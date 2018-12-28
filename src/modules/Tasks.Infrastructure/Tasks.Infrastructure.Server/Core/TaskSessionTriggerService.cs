using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Maze.Server.Data.EfCode;
using Maze.Utilities;
using Tasks.Infrastructure.Management.Data;
using Tasks.Infrastructure.Server.Library;

namespace Tasks.Infrastructure.Server.Core
{
    public class TaskSessionTriggerService : TaskSessionTrigger
    {
        private readonly MazeTaskService _taskService;

        public TaskSessionTriggerService(MazeTaskService taskService, TaskSession taskSession)
        {
            _taskService = taskService;
            Info = taskSession;
        }

        public override TaskSession Info { get; }

        public override async Task Invoke()
        {
            using (var serviceScope = _taskService.Services.CreateScope())
            {
                var dbContext = serviceScope.ServiceProvider.GetRequiredService<AppDbContext>();
                var clients = await dbContext.Clients.Select(x => x.ClientId).ToListAsync();

                //the filter is applied on execute so we just feed it with everything we have
                await _taskService.Execute(clients.Select(x => new TargetId(x)).Concat(TargetId.ServerId.Yield()), Info, CancellationToken.None);
            }
        }

        public override Task InvokeClient(int clientId)
        {
            return _taskService.Execute(new TargetId(clientId).Yield(), Info, CancellationToken.None);
        }

        public override Task InvokeServer()
        {
            return _taskService.Execute(TargetId.ServerId.Yield(), Info, CancellationToken.None);
        }
    }
}