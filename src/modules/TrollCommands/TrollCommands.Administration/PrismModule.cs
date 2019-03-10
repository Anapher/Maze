using System;
using System.Linq;
using System.Reflection;
using Maze.Administration.Library.Unity;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Unity;
using Tasks.Infrastructure.Administration.Library;
using Tasks.Infrastructure.Core.Commands;
using Tasks.Infrastructure.Core.Filter;
using Tasks.Infrastructure.Core.StopEvents;
using Tasks.Infrastructure.Core.Triggers;
using Unclassified.TxLib;
using Unity.RegistrationByConvention;

namespace TrollCommands.Administration
{
    public class PrismModule : IModule
    {
        public const string CommandsNamespace = "Fun/Troll";

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            Tx.LoadFromEmbeddedResource("TrollCommands.Administration.Resources.TrollCommands.Translation.txd");

            var unityContainer = containerRegistry.GetContainer();
            var currentAssembly = Assembly.GetExecutingAssembly();

            unityContainer.RegisterAssemblyTypesAsImplementedInterfaces<ITaskServiceDescription>(currentAssembly, WithLifetime.Transient);

            unityContainer.RegisterTypes(
                currentAssembly.ExportedTypes.Where(type => IsTypeImplementingInterface(type, typeof(ITaskServiceViewModel<>))),
                type => type.GetInterfaces().Where(x => IsTypeImplementingInterface(x, typeof(ITaskServiceViewModel<>))), null,
                WithLifetime.Transient);

            //RegisterTaskDtos
            unityContainer.RegisterAssemblyTypes<CommandInfo>(currentAssembly, WithLifetime.Transient);
            unityContainer.RegisterAssemblyTypes<TriggerInfo>(currentAssembly, WithLifetime.Transient);
            unityContainer.RegisterAssemblyTypes<FilterInfo>(currentAssembly, WithLifetime.Transient);
            unityContainer.RegisterAssemblyTypes<StopEventInfo>(currentAssembly, WithLifetime.Transient);
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
        }

        private static bool IsTypeImplementingInterface(Type sourceType, Type genericInterface)
        {
            return sourceType.GetInterfaces().Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == genericInterface);
        }
    }
}
