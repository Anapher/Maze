using System.Linq;
using System.Reflection;
using Maze.Administration.Library.Unity;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Unity;
using Tasks.Common.Administration.Resources;
using Tasks.Common.Administration.ViewProvider;
using Tasks.Infrastructure.Administration.Library;
using Tasks.Infrastructure.Core.Commands;
using Tasks.Infrastructure.Core.Filter;
using Tasks.Infrastructure.Core.StopEvents;
using Tasks.Infrastructure.Core.Triggers;
using Unclassified.TxLib;
using Unity.Lifetime;
using Unity.RegistrationByConvention;

namespace Tasks.Common.Administration
{
    public class PrismModule : IModule
    {
        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            Tx.LoadFromEmbeddedResource("Tasks.Common.Administration.Resources.Tasks.Common.Translation.txd");

            containerRegistry.RegisterSingleton<VisualStudioIcons>();

            var unityContainer = containerRegistry.GetContainer();
            var currentAssembly = Assembly.GetCallingAssembly();

            unityContainer.RegisterAssemblyTypesAsImplementedInterfaces<ITaskServiceDescription>(currentAssembly, WithLifetime.Transient);
            unityContainer.RegisterTypes(
                currentAssembly.ExportedTypes.Where(type =>
                    type.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ITaskServiceViewModel<>))),
                type => type.GetInterfaces(), null, WithLifetime.Transient);

            unityContainer.AsImplementedInterfaces<CommonViewProvider>(new TransientLifetimeManager());

            //RegisterTaskDtos
            unityContainer.RegisterAssemblyTypes<CommandInfo>(currentAssembly, WithLifetime.Transient);
            unityContainer.RegisterAssemblyTypes<TriggerInfo>(currentAssembly, WithLifetime.Transient);
            unityContainer.RegisterAssemblyTypes<FilterInfo>(currentAssembly, WithLifetime.Transient);
            unityContainer.RegisterAssemblyTypes<StopEventInfo>(currentAssembly, WithLifetime.Transient);
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
        }
    }
}