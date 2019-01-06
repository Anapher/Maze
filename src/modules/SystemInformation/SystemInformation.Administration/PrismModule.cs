using SystemInformation.Administration.Resources;
using SystemInformation.Administration.ViewModels;
using Maze.Administration.Library.Menus;
using Maze.Administration.Library.Models;
using Maze.Administration.Library.Services;
using Prism.Ioc;
using Prism.Modularity;
using Unclassified.TxLib;

namespace SystemInformation.Administration
{
    public class PrismModule : IModule
    {
        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            Tx.LoadFromEmbeddedResource("SystemInformation.Administration.Resources.SystemInformation.Translation.txd");
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            var registar = containerProvider.Resolve<IClientCommandRegistrar>();
            var icons = containerProvider.Resolve<VisualStudioIcons>();

            registar.Register<SystemInformationViewModel>("SystemInformation:SystemInformation", IconFactory.FromFactory(() => icons.SystemInfo),
                CommandCategory.System);
        }
    }
}