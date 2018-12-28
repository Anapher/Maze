using Autofac;

namespace Maze.ModuleManagement
{
    public class ModuleManagementModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterGeneric(typeof(ActionInterfaceInvoker<>)).As(typeof(IActionInterfaceInvoker<>))
                .InstancePerDependency();
            builder.RegisterGeneric(typeof(ActionInterfaceInvoker<,>)).As(typeof(IActionInterfaceInvoker<,>))
                .InstancePerDependency();
        }
    }
}