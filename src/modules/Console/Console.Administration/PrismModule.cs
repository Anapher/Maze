using Console.Administration.Resources;
using Console.Administration.ViewModels;
using Maze.Administration.Library.Menus;
using Maze.Administration.Library.Models;
using Maze.Administration.Library.Services;
using Prism.Ioc;
using Prism.Modularity;
using Unclassified.TxLib;

namespace Console.Administration
{
    public class PrismModule : IModule
    {
        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            Tx.LoadFromEmbeddedResource("Console.Administration.Resources.Console.Translation.txd");
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            var registrar = containerProvider.Resolve<IClientCommandRegistrar>();
            var icons = containerProvider.Resolve<VisualStudioIcons>();

            registrar.Register<ConsoleViewModel>("Console:Name", IconFactory.FromFactory(() => icons.Console), CommandCategory.System);
        }
    }
}