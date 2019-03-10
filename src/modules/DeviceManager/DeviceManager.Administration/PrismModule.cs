using DeviceManager.Administration.Resources;
using DeviceManager.Administration.ViewModels;
using Maze.Administration.Library.Menus;
using Maze.Administration.Library.Models;
using Maze.Administration.Library.Services;
using Prism.Ioc;
using Prism.Modularity;
using Unclassified.TxLib;

namespace DeviceManager.Administration
{
    public class PrismModule : IModule
    {
        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            Tx.LoadFromEmbeddedResource("DeviceManager.Administration.Resources.DeviceManager.Translation.txd");
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            var registrar = containerProvider.Resolve<IClientCommandRegistrar>();
            var icons = containerProvider.Resolve<VisualStudioIcons>();

            registrar.Register<DeviceManagerViewModel>("DeviceManager:Name", IconFactory.FromFactory(() => icons.Icon), CommandCategory.System);
        }
    }
}