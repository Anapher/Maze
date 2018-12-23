using System.Linq;
using Autofac;
using Tasks.Common.Administration.Resources;
using Tasks.Common.Administration.ViewProvider;
using Tasks.Infrastructure.Administration.Library;
using Tasks.Infrastructure.Core.Utilities;
using Unclassified.TxLib;

namespace Tasks.Common.Administration
{
    public class AutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            Tx.LoadFromEmbeddedResource("Tasks.Common.Administration.Resources.Tasks.Common.Translation.txd");

            builder.RegisterType<VisualStudioIcons>().SingleInstance();
            builder.RegisterAssemblyTypes(ThisAssembly).AssignableTo<ITaskServiceDescription>().AsImplementedInterfaces();
            builder.RegisterAssemblyTypes(ThisAssembly)
                .Where(x => x.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ITaskServiceViewModel<>)))
                .AsImplementedInterfaces();
            builder.RegisterType<CommonViewProvider>().AsImplementedInterfaces();
            builder.RegisterTaskDtos(ThisAssembly);
        }
    }
}