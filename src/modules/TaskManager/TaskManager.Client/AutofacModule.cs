using Autofac;
using TaskManager.Client.ProcessInfo;

namespace TaskManager.Client
{
    public class AutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterAssemblyTypes(ThisAssembly).AssignableTo<IProcessValueProvider>().AsImplementedInterfaces();
        }
    }
}