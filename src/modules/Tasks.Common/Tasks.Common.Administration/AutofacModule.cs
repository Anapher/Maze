using System.Linq;
using Autofac;
using Tasks.Common.Administration.Resources;
using Tasks.Infrastructure.Administration.Library.Command;

namespace Tasks.Common.Administration
{
    public class AutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterType<VisualStudioIcons>().SingleInstance();
            builder.RegisterAssemblyTypes(ThisAssembly).Where(x => typeof(ICommandDescription).IsAssignableFrom(x)).As<ICommandDescription>();
            builder.RegisterAssemblyTypes(ThisAssembly)
                .Where(x => x.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ICommandViewModel<>)))
                .AsImplementedInterfaces();
        }
    }
}