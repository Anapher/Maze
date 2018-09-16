using Autofac;
using RemoteDesktop.Client.Capture;
using RemoteDesktop.Client.Encoder;

namespace RemoteDesktop.Client
{
    public class AutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterAssemblyTypes(ThisAssembly).AssignableTo<IStreamEncoder>().AsImplementedInterfaces();
            builder.RegisterAssemblyTypes(ThisAssembly).AssignableTo<IScreenCaptureService>().AsImplementedInterfaces();
        }
    }
}