using Prism.Modularity;
using Unclassified.TxLib;

namespace Orcus.Administration.ViewModels
{
    public class PrismModule : IModule
    {
        public void Initialize()
        {
            Tx.LoadFromEmbeddedResource("Orcus.Administration.ViewModels.Resources.translation.txd");
        }
    }
}