using Autofac;
using Tasks.Infrastructure.Administration.Controls.PropertyGrid;
using Tasks.Infrastructure.Administration.Core;

namespace Tasks.Infrastructure.Administration.Hooks
{
    public class AutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterAssemblyTypes(ThisAssembly).AssignableTo<IPropertyEditorFactory>().As<IPropertyEditorFactory>();
            builder.RegisterType<DefaultPropertyEditorFinder>().As<IPropertyEditorFinder>();
            builder.RegisterType<DefaultViewProvider>().AsImplementedInterfaces();
            builder.RegisterType<PropertyGridViewProvider>().AsImplementedInterfaces();
        }
    }
}