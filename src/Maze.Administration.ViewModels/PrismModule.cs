using Maze.Administration.Core.Clients;
using Maze.Administration.Library.Services;
using Maze.Server.Connection.Utilities;
using Prism.Ioc;
using Prism.Modularity;
using Unclassified.TxLib;

namespace Maze.Administration.ViewModels
{
    public class PrismModule : IModule
    {
        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            Tx.LoadFromEmbeddedResource("Maze.Administration.ViewModels.Resources.translation.txd");

            containerRegistry.RegisterSingleton<IClientManager, ClientManager>();
            containerRegistry.RegisterSingleton<IXmlSerializerCache, XmlSerializerCache>();
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
        }
    }
}