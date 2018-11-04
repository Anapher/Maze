using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Orcus.Server.Library.Interfaces;
using Tasks.Infrastructure.Server.Data;

namespace Tasks.Infrastructure.Server.Hooks
{
    public class OnServerStartupEvent : IConfigureServerPipelineAction
    {
        private readonly OrcusTaskManager _orcusTaskManager;

        public OnServerStartupEvent(OrcusTaskManager orcusTaskManager)
        {
            _orcusTaskManager = orcusTaskManager;
        }

        public Task Execute(PipelineInfo context)
        {
            using (var scope = context.ApplicationBuilder.ApplicationServices.CreateScope())
            {
                var appContext = scope.ServiceProvider.GetRequiredService<TasksDbContext>();
                appContext.Database.Migrate();
            }
            return _orcusTaskManager.Initialize();
        }
    }
}