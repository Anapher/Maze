using Autofac;
using Tasks.Infrastructure.Administration.Controls.PropertyGrid;
using Tasks.Infrastructure.Administration.Core;
using Tasks.Infrastructure.Administration.Library.Result;
using Tasks.Infrastructure.Administration.Resources;
using Tasks.Infrastructure.Core;

namespace Tasks.Infrastructure.Administration.Hooks
{
    public class AutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterType<VisualStudioIcons>().SingleInstance();
            builder.RegisterAssemblyTypes(ThisAssembly).AssignableTo<IPropertyEditorFactory>().As<IPropertyEditorFactory>();
            builder.RegisterType<DefaultPropertyEditorFinder>().As<IPropertyEditorFinder>();
            builder.RegisterType<DefaultViewProvider>().AsImplementedInterfaces();
            builder.RegisterType<PropertyGridViewProvider>().AsImplementedInterfaces();
            builder.RegisterType<TaskComponentResolver>().As<ITaskComponentResolver>().SingleInstance();
            builder.RegisterAssemblyTypes(ThisAssembly).AssignableTo<ICommandResultViewProvider>().As<ICommandResultViewProvider>().SingleInstance();
            builder.RegisterType<CommandResultViewFactory>().As<ICommandResultViewFactory>();
            builder.RegisterType<CommandExecutionManager>().SingleInstance();
            builder.RegisterType<TaskActivityWatcher>();
        }
    }
}