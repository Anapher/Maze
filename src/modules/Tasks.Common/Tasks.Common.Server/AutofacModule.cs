using Autofac;
using Tasks.Infrastructure.Administration.Utilities;

namespace Tasks.Common.Server
{
    public class AutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            
            builder.RegisterTaskDtos(ThisAssembly);
            builder.RegisterAssemblyTypes(ThisAssembly).AsImplementedInterfaces();
        }
    }
}