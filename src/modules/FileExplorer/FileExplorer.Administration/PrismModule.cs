using System.Collections.Generic;
using FileExplorer.Administration.Resources;
using FileExplorer.Administration.Views;
using Orcus.Administration.Library.Menu;
using Orcus.Administration.Library.Services;
using Prism.Modularity;
using Unclassified.TxLib;

namespace FileExplorer.Administration
{
    public class PrismModule : IModule
    {
        private readonly VisualStudioIcons _icons;
        private readonly IEnumerable<ContextMenuManager> _menuBuilders;
        private readonly IClientCommandRegistrar _registrar;

        public PrismModule(IClientCommandRegistrar registrar, IEnumerable<ContextMenuManager> menuBuilders,
            VisualStudioIcons icons)
        {
            _registrar = registrar;
            _menuBuilders = menuBuilders;
            _icons = icons;
        }

        public void Initialize()
        {
            Tx.LoadFromEmbeddedResource("FileExplorer.Administration.Resources.FileExplorer.Translation.txd");

            _registrar.RegisterView(typeof(FileExplorerView), "FileExplorer:FileExplorer", _icons.ListFolder,
                CommandCategory.System);

            foreach (var contextMenuBuilder in _menuBuilders)
                contextMenuBuilder.Build();
        }
    }
}