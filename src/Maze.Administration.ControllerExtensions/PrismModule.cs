using CodeElements.NetworkCall.NetSerializer;
using Prism.Ioc;
using Prism.Modularity;

namespace Maze.Administration.ControllerExtensions
{
    public class PrismModule : IModule
    {
        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<NetSerializerNetworkSerializer>();
            containerRegistry.Register(typeof(CallTransmissionChannel<>), typeof(CallTransmissionChannel<>));
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
        }
    }
}