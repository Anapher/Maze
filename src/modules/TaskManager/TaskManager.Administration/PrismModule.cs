using Maze.Administration.Library.Menus;
using Maze.Administration.Library.Models;
using Maze.Administration.Library.Services;
using Prism.Ioc;
using Prism.Modularity;
using TaskManager.Administration.Resources;
using TaskManager.Administration.ViewModels;
using Unclassified.TxLib;

namespace TaskManager.Administration
{
    public class PrismModule : IModule
    {
        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            Tx.LoadFromEmbeddedResource("TaskManager.Administration.Resources.TaskManager.Translation.txd");

            containerRegistry.RegisterSingleton<VisualStudioIcons>();
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            var registrar = containerProvider.Resolve<IClientCommandRegistrar>();
            var icons = containerProvider.Resolve<VisualStudioIcons>();

            registrar.Register<TaskManagerViewModel>("TaskManager:TaskManager", IconFactory.FromFactory(() => icons.Process),
                CommandCategory.System);
        }
    }
}