using Maze.Administration.Core.Clients;
using Maze.Administration.Core.Rest;
using Maze.Administration.Library.Services;
using Maze.Administration.Library.Unity;
using Maze.Server.Connection.Utilities;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Unity;
using Unclassified.TxLib;
using Unity.Lifetime;

namespace Maze.Administration.ViewModels
{
    public class PrismModule : IModule
    {
        public const string MainContent = nameof(MainContent);
        public const string MainContentLoginView = "LoginView";
        public const string MainContentOverviewView = "OverviewView";

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            Tx.LoadFromEmbeddedResource("Maze.Administration.ViewModels.Resources.translation.txd");

            containerRegistry.RegisterSingleton<IClientManager, ClientManager>();
            containerRegistry.RegisterSingleton<IXmlSerializerCache, XmlSerializerCache>();
            containerRegistry.GetContainer().AsImplementedInterfaces<MazeRestClientWrapper, ContainerControlledLifetimeManager>();
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
        }
    }
}