using Autofac;
using CodeElements.NetworkCall.NetSerializer;

namespace Orcus.Administration.ControllerExtensions
{
    public class AutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.RegisterType<NetSerializerNetworkSerializer>().SingleInstance();
            builder.RegisterType(typeof(CallTransmissionChannel<>));
        }
    }
}