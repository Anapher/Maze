using System.Collections.Generic;
using System.IO;
using System.Windows;
using FileExplorer.Administration.Resources;
using FileExplorer.Administration.Rest;
using FileExplorer.Administration.ViewModels;
using FileExplorer.Administration.ViewModels.Explorer;
using FileExplorer.Administration.ViewModels.Explorer.Base;
using Maze.Administration.Library.Extensions;
using Ookii.Dialogs.Wpf;
using Maze.Administration.Library.Menu;
using Maze.Administration.Library.Menu.MenuBase;
using Maze.Administration.Library.StatusBar;
using Maze.Administration.Library.ViewModels;
using Maze.Utilities;
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
            FileExplorerResource.CopyPathToClipboard(viewModel.Source.Path, context.RestClient)
                .DisplayOnStatusBarCatchErrors(context.StatusBar, Tx.T("FileExplorer:CopyPathOnRemoteComputer"),
                    StatusBarAnimation.Save, true).Forget();
        }

        public static void DeleteEntries(IEnumerable<EntryViewModel> entries, FileExplorerViewModel context)
        {
            context.EntriesViewModel.RemoveEntries(entries).Forget();
        }

        public static void DownloadEntries(IEnumerable<EntryViewModel> entries, FileExplorerViewModel context)
        {
            var fbd = new VistaFolderBrowserDialog();

            var downloadsDirectory = Path.Combine(Directory.GetCurrentDirectory(), "downloads");
            Directory.CreateDirectory(downloadsDirectory);

            fbd.SelectedPath = downloadsDirectory;

            if (context.Window.ShowDialog(fbd) != true)
                return;

            foreach (var entryViewModel in entries)
            {
                context.FileTransferManagerViewModel.ExecuteTransfer(new FileTransferViewModel(entryViewModel,
                    Path.Combine(fbd.SelectedPath, entryViewModel.Label)));
            }
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
                Command = new ContextDelegateCommand<DirectoryViewModel, FileExplorerViewModel>((model, viewModel) => EntryViewModelCommands.DownloadEntries(model.Yield(), viewModel)),
                MultipleCommand = new ContextDelegateCommand<IEnumerable<EntryViewModel>, FileExplorerViewModel>(EntryViewModelCommands.DownloadEntries)
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
                Command = new ContextDelegateCommand<DirectoryViewModel, FileExplorerViewModel>(OpenDirectoryProperties)
            });
        }

        private void OpenDirectoryProperties(DirectoryViewModel directory, FileExplorerViewModel context)
        {
            context.Window.Show<PropertiesViewModel>(null, window =>
            {
                window.Title = Tx.T("FileExplorer:Properties.Title", "name", directory.Name);
            }, viewModel => viewModel.Initialize(directory), out _);
        }

        protected override IEnumerable<UIElement> GetItems(object context) =>
            _menuFactory.Create(_contextMenu, context);
    }
}