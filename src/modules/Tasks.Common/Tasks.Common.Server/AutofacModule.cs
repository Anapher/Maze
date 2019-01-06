using Autofac;
using Tasks.Common.Server.Triggers;
using Tasks.Infrastructure.Management.Utilities;

namespace Tasks.Common.Server
{
    public class AutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterTaskDtos(ThisAssembly);
            builder.RegisterAssemblyTypes(ThisAssembly).AsImplementedInterfaces();
            builder.RegisterType<ClientEventNotifier>().AsSelf().As<IClientEventNotifier>().SingleInstance();
        }
    }
}