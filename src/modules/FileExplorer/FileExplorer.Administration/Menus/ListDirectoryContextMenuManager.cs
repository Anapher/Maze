using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using FileExplorer.Administration.Resources;
using FileExplorer.Administration.ViewModels;
using FileExplorer.Administration.ViewModels.Explorer;
using FileExplorer.Administration.ViewModels.Explorer.Base;
using Orcus.Administration.Library.Menu;
using Orcus.Administration.Library.Menu.MenuBase;
using Orcus.Administration.Library.StatusBar;
using Orcus.Administration.Library.ViewModels;
using Orcus.Utilities;
using Unclassified.TxLib;

namespace FileExplorer.Administration.Menus
{
    public static class EntryViewModelCommands
    {
        public static void CopyPath(EntryViewModel viewModel, FileExplorerViewModel context)
        {
            Clipboard.SetText(viewModel.Source.Path);
        }

        public static void CopyName(EntryViewModel viewModel, FileExplorerViewModel context)
        {
            Clipboard.SetText(viewModel.Name);
        }

        public static void CopyParentFolderPath(EntryViewModel viewModel, FileExplorerViewModel context)
        {
            Clipboard.SetText(context.CurrentPath);
        }

        public static void CopyPathOnRemoteComputer(EntryViewModel viewModel, FileExplorerViewModel context)
        {
            Clipboard.SetText(context.CurrentPath);
        }

        public static void DeleteEntries(IEnumerable<EntryViewModel> entries, FileExplorerViewModel context)
        {
            context.EntriesViewModel.RemoveEntries(entries).Forget();
        }

        public static void OpenProperties(EntryViewModel viewModel, FileExplorerViewModel context)
        {

        }

        public static void Rename(EntryViewModel viewModel, FileExplorerViewModel context)
        {
            context.EntriesViewModel.EnterNameEditingCommand.Execute(viewModel);
        }
    }

    public class ListDirectoryContextMenuManager : ContextMenuManager
    {
        private readonly FileExplorerListDirectoryContextMenu _contextMenu;
        private readonly VisualStudioIcons _icons;
        private readonly IItemMenuFactory _menuFactory;

        public ListDirectoryContextMenuManager(FileExplorerListDirectoryContextMenu contextMenu,
            IItemMenuFactory menuFactory, VisualStudioIcons icons)
        {
            _contextMenu = contextMenu;
            _menuFactory = menuFactory;
            _icons = icons;
        }

        public override void Build()
        {
            _contextMenu.Section1Path.Header = Tx.T("FileExplorer:PathCopy");
            _contextMenu.Section1Path.Icon = _icons.CopyToClipboard;
            _contextMenu.Section1.AddAtBeginning(new ContextDiItemsCommand<DirectoryViewModel, EntryViewModel, FileExplorerViewModel>
            {
                Header = Tx.T("FileExplorer:CopyDirectoryPath"),
                Command = new ContextDelegateCommand<DirectoryViewModel, FileExplorerViewModel>(EntryViewModelCommands.CopyPath)
            });
            _contextMenu.Section1Path.Add(new ContextDiItemsCommand<DirectoryViewModel, EntryViewModel, FileExplorerViewModel>
            {
                Header = Tx.T("Name"),
                Command = new ContextDelegateCommand<DirectoryViewModel, FileExplorerViewModel>(EntryViewModelCommands.CopyName)
            });
            _contextMenu.Section1Path.Add(new ContextDiItemsCommand<DirectoryViewModel, EntryViewModel, FileExplorerViewModel>
            {
                Header = Tx.T("FileExplorer:ParentFolderPath"),
                Command = new ContextDelegateCommand<DirectoryViewModel, FileExplorerViewModel>(EntryViewModelCommands.CopyParentFolderPath)
            });
            _contextMenu.Section1Path.Add(new MenuSection<ContextDiItemsCommand<DirectoryViewModel, EntryViewModel, FileExplorerViewModel>>
            {
                new ContextDiItemsCommand<DirectoryViewModel, EntryViewModel, FileExplorerViewModel>
                {
                    Header = Tx.T("FileExplorer:CopyPathOnRemoteComputer"),
                    Command = new ContextDelegateCommand<DirectoryViewModel, FileExplorerViewModel>(EntryViewModelCommands.CopyPathOnRemoteComputer)
                }
            });

            _contextMenu.Section2Archive.Header = Tx.T("FileExplorer:Archive");
            _contextMenu.Section2Archive.Icon = _icons.ZipFile;
            _contextMenu.Section2.Add(new ContextDiItemsCommand<DirectoryViewModel, EntryViewModel, FileExplorerViewModel>
            {
                Header = Tx.T("FileExplorer:Download"),
                Icon = _icons.DownloadFile,
                Command = new ContextDelegateCommand<DirectoryViewModel, FileExplorerViewModel>(DownloadDirectory)
            });
            _contextMenu.Section3.Add(new ContextDiItemsCommand<DirectoryViewModel, EntryViewModel, FileExplorerViewModel>
            {
                Header = Tx.T("FileExplorer:Remove"),
                Icon = _icons.Cancel,
                Command = new ContextDelegateCommand<DirectoryViewModel, FileExplorerViewModel>((model, viewModel) => EntryViewModelCommands.DeleteEntries(model.Yield(), viewModel)),
                MultipleCommand = new ContextDelegateCommand<IEnumerable<EntryViewModel>, FileExplorerViewModel>(EntryViewModelCommands.DeleteEntries)
            });
            _contextMenu.Section3.Add(new ContextDiItemsCommand<DirectoryViewModel, EntryViewModel, FileExplorerViewModel>
            {
                Header = Tx.T("FileExplorer:Rename"),
                Icon = _icons.Rename,
                Command = new ContextDelegateCommand<DirectoryViewModel, FileExplorerViewModel>(EntryViewModelCommands.Rename)
            });
            _contextMenu.Section4.Add(new ContextDiItemsCommand<DirectoryViewModel, EntryViewModel, FileExplorerViewModel>
            {
                Header = Tx.T("FileExplorer:Properties"),
                Icon = _icons.Property,
                Command = new ContextDelegateCommand<DirectoryViewModel, FileExplorerViewModel>(EntryViewModelCommands.OpenProperties)
            });
        }

        private void DownloadDirectory(DirectoryViewModel arg1, FileExplorerViewModel arg2)
        {
            throw new System.NotImplementedException();
        }

        protected override IEnumerable<UIElement> GetItems(object context) =>
            _menuFactory.Create(_contextMenu, context);
    }
}