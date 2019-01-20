using System.Reflection;
using Maze.Administration.Library;
using Maze.Administration.Library.Unity;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;
using Prism.Unity;
using Tasks.Infrastructure.Administration.Controls.PropertyGrid;
using Tasks.Infrastructure.Administration.Core;
using Tasks.Infrastructure.Administration.Library.Result;
using Tasks.Infrastructure.Administration.Resources;
using Tasks.Infrastructure.Administration.Views;
using Tasks.Infrastructure.Core;
using Unclassified.TxLib;
using Unity;
using Unity.Lifetime;
using Unity.RegistrationByConvention;

namespace Tasks.Infrastructure.Administration.Hooks
{
    public class PrismModule : IModule
    {
        public const string ModuleName = "Tasks.Infrastructure";

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            Tx.LoadFromEmbeddedResource("Tasks.Infrastructure.Administration.Resources.Tasks.Infrastructure.Translation.txd");

            var unityContainer = containerRegistry.GetContainer();
            var currentAssembly = Assembly.GetExecutingAssembly();

            unityContainer.RegisterAssemblyTypes<IPropertyEditorFactory>(currentAssembly, WithLifetime.Transient);
            unityContainer.RegisterType<IPropertyEditorFinder, DefaultPropertyEditorFinder>();
            unityContainer.AsImplementedInterfaces<DefaultViewProvider, TransientLifetimeManager>(nameof(DefaultViewProvider));
            unityContainer.AsImplementedInterfaces<PropertyGridViewProvider, TransientLifetimeManager>(nameof(PropertyGridViewProvider));
            unityContainer.RegisterSingleton<ITaskComponentResolver, TaskComponentResolver>();
            unityContainer.RegisterAssemblyTypes<ICommandResultViewProvider>(currentAssembly, WithLifetime.ContainerControlled);
            unityContainer.RegisterType<ICommandResultViewFactory, CommandResultViewFactory>();
            unityContainer.RegisterSingleton<CommandExecutionManager>();
            unityContainer.RegisterType<TaskActivityWatcher>();
            unityContainer.RegisterSingleton<VisualStudioIcons>();
            unityContainer.RegisterType<ContextMenuInitalizer>();
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            var regionManager = containerProvider.Resolve<IRegionManager>();
            regionManager.RegisterViewWithRegion(RegionNames.OverviewTabs, typeof(TasksView));

            containerProvider.Resolve<ContextMenuInitalizer>().Initalize();
        }
    }
}