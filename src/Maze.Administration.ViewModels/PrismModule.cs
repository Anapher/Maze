using Prism.Modularity;
using Unclassified.TxLib;

namespace Maze.Administration.ViewModels
{
    public class PrismModule : IModule
    {
        public void Initialize()
        {
            Tx.LoadFromEmbeddedResource("Maze.Administration.ViewModels.Resources.translation.txd");
        }
    }
}