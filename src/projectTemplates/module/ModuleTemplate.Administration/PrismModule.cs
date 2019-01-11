using ModuleTemplate.Administration.Resources;
using ModuleTemplate.Administration.ViewModels;
using Maze.Administration.Library.Menus;
using Maze.Administration.Library.Models;
using Maze.Administration.Library.Services;
using Prism.Ioc;
using Prism.Modularity;
using Unclassified.TxLib;

namespace ModuleTemplate.Administration
{
    public class PrismModule : IModule
    {
        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            Tx.LoadFromEmbeddedResource("$safeprojectname$.Resources.Module.Translation.txd");
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            var registrar = containerProvider.Resolve<IClientCommandRegistrar>();
            var icons = containerProvider.Resolve<VisualStudioIcons>();

            registrar.Register<ModuleViewModel>("Module:Name", IconFactory.FromFactory(() => icons.Icon), CommandCategory.System);
        }
    }
}