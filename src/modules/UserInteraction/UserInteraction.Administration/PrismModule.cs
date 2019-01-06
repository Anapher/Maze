using Maze.Administration.Library.Menus;
using Maze.Administration.Library.Models;
using Maze.Administration.Library.Services;
using Prism.Ioc;
using Prism.Modularity;
using Unclassified.TxLib;
using UserInteraction.Administration.Resources;
using UserInteraction.Administration.ViewModels;

namespace UserInteraction.Administration
{
    public class PrismModule : IModule
    {
        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            Tx.LoadFromEmbeddedResource("UserInteraction.Administration.Resources.UserInteraction.Translation.txd");

            containerRegistry.RegisterSingleton<VisualStudioIcons>();
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            var registar = containerProvider.Resolve<IClientCommandRegistrar>();
            var icons = containerProvider.Resolve<VisualStudioIcons>();

            registar.Register<MessageBoxViewModel>("UserInteraction:MessageBox", IconFactory.FromFactory(() => icons.MessageBox),
                CommandCategory.Interaction);
        }
    }
}