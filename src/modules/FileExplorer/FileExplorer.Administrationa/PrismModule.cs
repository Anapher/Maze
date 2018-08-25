using FileExplorer.Administration.Views;
using Orcus.Administration.Library.Services;
using Prism.Modularity;
using Unclassified.TxLib;

namespace FileExplorer.Administration
{
    public class PrismModule : IModule
    {
        private readonly IClientCommandRegistrar _registrar;

        public PrismModule(IClientCommandRegistrar registrar)
        {
            _registrar = registrar;
        }

        public void Initialize()
        {
            Tx.LoadFromEmbeddedResource("FileExplorer.Administration.Resources.FileExplorer.Translation.txd");

            _registrar.RegisterView(typeof(FileExplorerView), "FileExplorer:FileExplorer",
                null, CommandCategory.System);
        }
    }
}