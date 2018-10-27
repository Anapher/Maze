using System.Threading.Tasks;
using Orcus.Server.Library.Interfaces;

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
            return _orcusTaskManager.Initialize();
        }
    }
}