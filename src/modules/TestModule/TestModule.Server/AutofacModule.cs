using Autofac;

namespace TestModule.Server
{
    public class AutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.RegisterAssemblyTypes(GetType().Assembly).AsImplementedInterfaces();
        }
    }
}