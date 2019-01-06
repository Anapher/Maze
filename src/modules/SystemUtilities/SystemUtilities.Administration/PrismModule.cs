using SystemUtilities.Administration.Resources;
using Maze.Administration.Library.Services;
using Prism.Ioc;
using Prism.Modularity;
using Unclassified.TxLib;

namespace SystemUtilities.Administration
{
    public class PrismModule : IModule
    {
        public const string ModuleName = "SystemUtilities";

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            Tx.LoadFromEmbeddedResource("SystemUtilities.Administration.Resources.SystemUtilities.Translation.txd");
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
        }
    }
}