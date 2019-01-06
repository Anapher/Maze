using Maze.Administration.Library.Menus;
using Maze.Administration.Library.Models;
using Maze.Administration.Library.Services;
using Prism.Ioc;
using Prism.Modularity;
using RemoteDesktop.Administration.Resources;
using RemoteDesktop.Administration.ViewModels;
using Unclassified.TxLib;

namespace RemoteDesktop.Administration
{
    public class PrismModule : IModule
    {
        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            Tx.LoadFromEmbeddedResource("RemoteDesktop.Administration.Resources.RemoteDesktop.Translation.txd");
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            var registar = containerProvider.Resolve<IClientCommandRegistrar>();
            var icons = containerProvider.Resolve<VisualStudioIcons>();

            registar.Register<RemoteDesktopViewModel>("RemoteDesktop:Name", IconFactory.FromFactory(() => icons.Monitor),
                CommandCategory.Surveillance);
        }
    }
}