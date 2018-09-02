using System.Collections.Generic;
using System.Windows;
using FileExplorer.Administration.Resources;
using FileExplorer.Administration.ViewModels;
using FileExplorer.Administration.ViewModels.Explorer;
using FileExplorer.Administration.ViewModels.Explorer.Base;
using Orcus.Administration.Library.Menu;
using Orcus.Administration.Library.Menu.MenuBase;
using Orcus.Administration.Library.ViewModels;
using Orcus.Utilities;
using Unclassified.TxLib;

namespace FileExplorer.Administration.Menus
{
    public class ListFileContextMenuManager : ContextMenuManager
    {
        private readonly FileExplorerListFileContextMenu _contextMenu;
        private readonly VisualStudioIcons _icons;
        private readonly IItemMenuFactory _menuFactory;

        public ListFileContextMenuManager(FileExplorerListFileContextMenu contextMenu, IItemMenuFactory menuFactory,
            VisualStudioIcons icons)
        {
            _contextMenu = contextMenu;
            _menuFactory = menuFactory;
            _icons = icons;
        }

        public override void Build()
        {
            _contextMenu.Section1.Add(new ContextDiItemsCommand<FileViewModel, EntryViewModel, FileExplorerViewModel>
            {
                Header = Tx.T("FileExplorer:Execute"),
                Icon = _icons.StartupProject,
                Command = new ContextDelegateCommand<FileViewModel, FileExplorerViewModel>(ExecuteFile)
            });

            _contextMenu.Section2Path.Header = Tx.T("FileExplorer:PathCopy");
            _contextMenu.Section2Path.Icon = _icons.CopyToClipboard;
            _contextMenu.Section2.AddAtBeginning(new ContextDiItemsCommand<FileViewModel, EntryViewModel, FileExplorerViewModel>
            {
                Header = Tx.T("FileExplorer:CopyFilePath"),
                Command = new ContextDelegateCommand<FileViewModel, FileExplorerViewModel>(EntryViewModelCommands.CopyPath)
            });
            _contextMenu.Section2Path.Add(new ContextDiItemsCommand<FileViewModel, EntryViewModel, FileExplorerViewModel>
            {
                Header = Tx.T("Name"),
                Command = new ContextDelegateCommand<FileViewModel, FileExplorerViewModel>(EntryViewModelCommands.CopyName)
            });
            _contextMenu.Section2Path.Add(new ContextDiItemsCommand<FileViewModel, EntryViewModel, FileExplorerViewModel>
            {
                Header = Tx.T("FileExplorer:ParentFolderPath"),
                Command = new ContextDelegateCommand<FileViewModel, FileExplorerViewModel>(EntryViewModelCommands.CopyParentFolderPath)
            });
            _contextMenu.Section2Path.Add(new MenuSection<ContextDiItemsCommand<FileViewModel, EntryViewModel, FileExplorerViewModel>>
            {
                new ContextDiItemsCommand<FileViewModel, EntryViewModel, FileExplorerViewModel>
                {
                    Header = Tx.T("FileExplorer:CopyPathOnRemoteComputer"),
                    Command = new ContextDelegateCommand<FileViewModel, FileExplorerViewModel>(EntryViewModelCommands.CopyPathOnRemoteComputer)
                }
            });

            _contextMenu.Section3Archive.Header = Tx.T("FileExplorer:Archive");
            _contextMenu.Section3Archive.Icon = _icons.ZipFile;
            _contextMenu.Section3.Add(new ContextDiItemsCommand<FileViewModel, EntryViewModel, FileExplorerViewModel>
            {
                Header = Tx.T("FileExplorer:Download"),
                Icon = _icons.DownloadFile,
                Command = new ContextDelegateCommand<FileViewModel, FileExplorerViewModel>(DownloadFile)
            });
            _contextMenu.Section4.Add(new ContextDiItemsCommand<FileViewModel, EntryViewModel, FileExplorerViewModel>
            {
                Header = Tx.T("FileExplorer:Remove"),
                Icon = _icons.Cancel,
                Command = new ContextDelegateCommand<FileViewModel, FileExplorerViewModel>((model, viewModel) => EntryViewModelCommands.DeleteEntries(model.Yield(), viewModel)),
                MultipleCommand = new ContextDelegateCommand<IEnumerable<EntryViewModel>, FileExplorerViewModel>(EntryViewModelCommands.DeleteEntries)
            });
            _contextMenu.Section4.Add(new ContextDiItemsCommand<FileViewModel, EntryViewModel, FileExplorerViewModel>
            {
                Header = Tx.T("FileExplorer:Rename"),
                Icon = _icons.Rename,
                Command = new ContextDelegateCommand<FileViewModel, FileExplorerViewModel>(EntryViewModelCommands.Rename)
            });
            _contextMenu.Section5.Add(new ContextDiItemsCommand<FileViewModel, EntryViewModel, FileExplorerViewModel>
            {
                Header = Tx.T("FileExplorer:Properties"),
                Icon = _icons.Property,
                Command = new ContextDelegateCommand<FileViewModel, FileExplorerViewModel>(EntryViewModelCommands.OpenProperties)
            });
        }

        private void DownloadFile(FileViewModel file, FileExplorerViewModel context)
        {
            context.FileTransferManagerViewModel.ExecuteTransfer(new FileTransferViewModel(file, "D:\\test.rar"));
        }

        private void ExecuteFile(FileViewModel file, FileExplorerViewModel context)
        {
            throw new System.NotImplementedException();
        }

        protected override IEnumerable<UIElement> GetItems(object context) =>
            _menuFactory.Create(_contextMenu, context);
    }
}