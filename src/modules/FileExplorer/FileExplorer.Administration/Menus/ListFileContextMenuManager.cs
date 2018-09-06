using System.Collections.Generic;
using System.IO;
using System.Windows;
using FileExplorer.Administration.Resources;
using FileExplorer.Administration.Rest;
using FileExplorer.Administration.ViewModels;
using FileExplorer.Administration.ViewModels.Explorer;
using FileExplorer.Administration.ViewModels.Explorer.Base;
using Microsoft.Win32;
using Orcus.Administration.Library.Extensions;
using Orcus.Administration.Library.Menu;
using Orcus.Administration.Library.Menu.MenuBase;
using Orcus.Administration.Library.Services;
using Orcus.Administration.Library.StatusBar;
using Orcus.Administration.Library.ViewModels;
using Orcus.Utilities;
using Unclassified.TxLib;

namespace FileExplorer.Administration.Menus
{
    public class ListFileContextMenuManager : ContextMenuManager
    {
        private readonly FileExplorerListFileContextMenu _contextMenu;
        private readonly VisualStudioIcons _icons;
        private readonly IWindowService _windowService;
        private readonly IItemMenuFactory _menuFactory;

        public ListFileContextMenuManager(FileExplorerListFileContextMenu contextMenu, IItemMenuFactory menuFactory,
            VisualStudioIcons icons, IWindowService windowService)
        {
            _contextMenu = contextMenu;
            _menuFactory = menuFactory;
            _icons = icons;
            _windowService = windowService;
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
                Command = new ContextDelegateCommand<FileViewModel, FileExplorerViewModel>(DownloadFile),
                MultipleCommand = new ContextDelegateCommand<IEnumerable<EntryViewModel>, FileExplorerViewModel>(EntryViewModelCommands.DownloadEntries)
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
                Command = new ContextDelegateCommand<FileViewModel, FileExplorerViewModel>(OpenFileProperties)
            });
        }

        private async void OpenFileProperties(FileViewModel file, FileExplorerViewModel context)
        {
            var properties = await FileSystemResource.GetFileProperties(file.Source.Path, context.RestClient)
                .DisplayOnStatusBarCatchErrors(context.StatusBar, Tx.T("FileExplorer:StatusBar.LoadProperties"));
            if (!properties.Failed)
            {
                _windowService.Show(new PropertiesViewModel(file, properties.Result),
                    Tx.T("FileExplorer:Properties.Title", "name", file.Name), context.Window,
                    window => window.ViewManager.TaskBarIcon = file.Icon, null);
            }
        }

        private void DownloadFile(FileViewModel file, FileExplorerViewModel context)
        {
            var sfd = new SaveFileDialog();

            var ext = Path.GetExtension(file.Name);
            sfd.Filter = ext != null
                ? $"{Tx.T("FileExplorer:OriginalFileExtension")}|*{ext}|{Tx.T("FileExplorer:AllFilesFilter")}"
                : Tx.T("FileExplorer:AllFilesFilter");
            sfd.OverwritePrompt = true;

            var downloadsDirectory = Path.Combine(Directory.GetCurrentDirectory(), "downloads");
            Directory.CreateDirectory(downloadsDirectory);
            sfd.CustomPlaces.Add(new FileDialogCustomPlace(downloadsDirectory));
            sfd.InitialDirectory = downloadsDirectory;
            sfd.FileName = file.Name;

            if (context.Window.ShowDialog(sfd) != true)
                return;
        
            context.FileTransferManagerViewModel.ExecuteTransfer(new FileTransferViewModel(file, sfd.FileName));
        }

        private void ExecuteFile(FileViewModel file, FileExplorerViewModel context)
        {
            throw new System.NotImplementedException();
        }

        protected override IEnumerable<UIElement> GetItems(object context) =>
            _menuFactory.Create(_contextMenu, context);
    }
}