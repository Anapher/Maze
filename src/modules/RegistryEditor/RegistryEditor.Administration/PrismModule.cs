using RegistryEditor.Administration.Resources;
using RegistryEditor.Administration.ViewModels;
using Maze.Administration.Library.Menus;
using Maze.Administration.Library.Models;
using Maze.Administration.Library.Services;
using Prism.Ioc;
using Prism.Modularity;
using Unclassified.TxLib;

namespace RegistryEditor.Administration
{
    public class PrismModule : IModule
    {
        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            Tx.LoadFromEmbeddedResource("RegistryEditor.Administration.Resources.RegistryEditor.Translation.txd");
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            var registar = containerProvider.Resolve<IClientCommandRegistrar>();
            var icons = containerProvider.Resolve<VisualStudioIcons>();

            registar.Register<RegistryEditorViewModel>("RegistryEditor:Name", IconFactory.FromFactory(() => icons.Registry), CommandCategory.System);
        }
    }
}