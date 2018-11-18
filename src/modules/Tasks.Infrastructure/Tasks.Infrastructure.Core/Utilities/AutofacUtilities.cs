using System.Reflection;
using Autofac;
using Tasks.Infrastructure.Core.Commands;
using Tasks.Infrastructure.Core.Filter;
using Tasks.Infrastructure.Core.StopEvents;
using Tasks.Infrastructure.Core.Triggers;

namespace Tasks.Infrastructure.Core.Utilities
{
    public static class AutofacUtilities
    {
        public static void RegisterTaskDtos(this ContainerBuilder builder, Assembly assembly)
        {
            builder.RegisterAssemblyTypes(assembly).AssignableTo<CommandInfo>().As<CommandInfo>();
            builder.RegisterAssemblyTypes(assembly).AssignableTo<TriggerInfo>().As<TriggerInfo>();
            builder.RegisterAssemblyTypes(assembly).AssignableTo<FilterInfo>().As<FilterInfo>();
            builder.RegisterAssemblyTypes(assembly).AssignableTo<StopEventInfo>().As<StopEventInfo>();
        }
    }
}