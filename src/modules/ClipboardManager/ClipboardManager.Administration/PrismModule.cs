using ClipboardManager.Administration.Utilities;
using ClipboardManager.Shared.Utilities;
using Prism.Ioc;
using Prism.Modularity;
using Unclassified.TxLib;

namespace ClipboardManager.Administration
{
    public class PrismModule : IModule
    {
        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            Tx.LoadFromEmbeddedResource("ClipboardManager.Administration.Resources.ClipboardManager.Translation.txd");

            containerRegistry.RegisterSingleton<ClipboardWatcher>();
            containerRegistry.Register<ClipboardSynchronizer>();
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
        }
    }
}