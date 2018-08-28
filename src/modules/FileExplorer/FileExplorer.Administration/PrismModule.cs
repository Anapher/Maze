using Autofac;
using FileExplorer.Administration.Menus;
using FileExplorer.Administration.Resources;
using FileExplorer.Administration.Views;
using Orcus.Administration.Library.Services;
using Prism.Modularity;
using Unclassified.TxLib;

namespace FileExplorer.Administration
{
    public class PrismModule : IModule
    {
        private readonly IClientCommandRegistrar _registrar;
        private readonly IComponentContext _scope;

        public PrismModule(IClientCommandRegistrar registrar, IComponentContext scope)
        {
            _registrar = registrar;
            _scope = scope;
        }

        public void Initialize()
        {
            Tx.LoadFromEmbeddedResource("FileExplorer.Administration.Resources.FileExplorer.Translation.txd");

            _registrar.RegisterView(typeof(FileExplorerView), "FileExplorer:FileExplorer",
                VisualStudioImages.ListFolder(), CommandCategory.System);

            InitializeFileExplorerContextMenu(_scope);
        }

        private void InitializeFileExplorerContextMenu(IComponentContext scope)
        {
            var contextMenu = scope.Resolve<FileExplorerContextMenu>();
            new FileExplorerContextMenuBuilder().Build(contextMenu);
        }
    }
}