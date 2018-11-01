using Autofac;
using Tasks.Infrastructure.Administration.Core;

namespace Tasks.Infrastructure.Administration
{
    public class AutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterType<DefaultViewProvider>().AsImplementedInterfaces();
        }
    }
}