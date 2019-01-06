using Autofac;
using Tasks.Infrastructure.Management.Utilities;

namespace Tasks.Common.Client
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