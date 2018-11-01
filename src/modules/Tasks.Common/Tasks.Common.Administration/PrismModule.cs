using Prism.Modularity;
using Unclassified.TxLib;

namespace Tasks.Common.Administration
{
    public class PrismModule : IModule
    {
        public void Initialize()
        {
            Tx.LoadFromEmbeddedResource("Tasks.Common.Administration.Resources.Tasks.Common.Translation.txd");
        }
    }
}