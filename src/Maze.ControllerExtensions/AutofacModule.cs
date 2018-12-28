using Autofac;
using CodeElements.NetworkCall.NetSerializer;

namespace Maze.ControllerExtensions
{
    public class AutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.RegisterType<NetSerializerNetworkSerializer>().SingleInstance();
        }
    }
}