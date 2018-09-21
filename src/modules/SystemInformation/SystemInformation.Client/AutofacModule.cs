using Autofac;

namespace SystemInformation.Client
{
    public class AutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterAssemblyTypes(ThisAssembly).AssignableTo<ISystemInfoProvider>().AsImplementedInterfaces();
        }
    }
}