using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using FileExplorer.Administration.Resources;
using FileExplorer.Administration.Utilities;
using FileExplorer.Administration.ViewModels;
using FileExplorer.Administration.ViewModels.Explorer;
using Ookii.Dialogs.Wpf;
using Maze.Administration.Library.Extensions;
using Maze.Administration.Library.Menu;
using Maze.Administration.Library.Menu.MenuBase;
using Maze.Administration.Library.StatusBar;
using Maze.Administration.Library.Views;
using Maze.Utilities;
using Prism.Commands;
using Unclassified.TxLib;

namespace FileExplorer.Administration.Menus
{
    public class FileExplorerContextMenuManager : ContextMenuManager
    {
        private readonly FileExplorerContextMenu _contextMenu;
        private readonly IImageProvider _imageProvider;
        private readonly IWindowService _windowService;
        private readonly IMenuFactory _menuFactory;
        private readonly VisualStudioIcons _visualStudioIcons;

        public FileExplorerContextMenuManager(FileExplorerContextMenu contextMenu, IMenuFactory menuFactory,
            VisualStudioIcons visualStudioIcons, IImageProvider imageProvider, IWindowService windowService)
        {
            _contextMenu = contextMenu;
            _menuFactory = menuFactory;
            _visualStudioIcons = visualStudioIcons;
            _imageProvider = imageProvider;
            _windowService = windowService;
        }

        public override void Build()
        {
            _contextMenu.Section1.Add(new ContextCommand<FileExplorerViewModel>
            {
                Header = Tx.T("FileExplorer:Refresh"),
                Icon = _visualStudioIcons.Refresh,
                Command = new DelegateCommand<FileExplorerViewModel>(RefreshDirectory)
            });

            _contextMenu.Section2.Add(new ContextCommand<FileExplorerViewModel>
            {
                Header = Tx.T("FileExplorer:UploadFile"),
                Icon = _visualStudioIcons.UploadFile,
                Command = new DelegateCommand<FileExplorerViewModel>(UploadFile)
            });
            //_contextMenu.Section2.Add(new ContextCommand<FileExplorerViewModel>
            //{
            //    Header = Tx.T("FileExplorer:DownloadFileToDirectory"),
            //    Icon = _visualStudioIcons.DownloadFile,
            //    Command = new DelegateCommand<FileExplorerViewModel>(DownloadFile)
            //});

            _contextMenu.Section3New.Header = Tx.T("FileExplorer:New");
            _contextMenu.Section3New.Add(new ContextCommand<FileExplorerViewModel>
            {
                Header = Tx.T("FileExplorer:Directory"),
                Icon = new Image
                {
                    Source = _imageProvider.GetFolderImage(null, 0),
                    Width = 18,
                    Height = 18,
                    Stretch = Stretch.UniformToFill
                },
                Command = new DelegateCommand<FileExplorerViewModel>(CreateDirectory)
            });
        }

        private static void RefreshDirectory(FileExplorerViewModel context) => context.EntriesViewModel.RefreshCommand.Execute();

        private void CreateDirectory(FileExplorerViewModel context)
        {
            if (context.Window.ShowDialog<InputTextViewModel>(Tx.T("FileExplorer:CreateNewFolder"), vm => {
                vm.Initialize(Tx.T("FileExplorer:NewFolder"), Tx.T("FileExplorer:FolderName"),
                        Tx.T("Create"));
                vm.Predicate = s => context.FileSystem.IsValidFilename(s); }, out var viewModel) == true)
            {
                context.FileSystem.CreateDirectory(Path.Combine(context.CurrentPath, viewModel.Text))
                    .DisplayOnStatusBarCatchErrors(context.StatusBar, Tx.T("FileExplorer:CreateNewFolder")).Forget();
            }
        }

        //private void DownloadFile(FileExplorerViewModel obj)
        //{
        //    throw new System.NotImplementedException();
        //}

        private void UploadFile(FileExplorerViewModel context)
        {
            var ofd = new VistaOpenFileDialog
            {
                Title = Tx.T("FileExplorer:SelectFilesToUpload"),
                Filter = Tx.T("FileExplorer:AllFilesFilter"),
                CheckFileExists = true,
                CheckPathExists = true,
                Multiselect = true
            };

            if (context.Window.ShowDialog(ofd) == true)
            {
                foreach (var fileInfo in ofd.FileNames.Select(x => new FileInfo(x)))
                    context.FileTransferManagerViewModel.ExecuteTransfer(
                        new FileTransferViewModel(fileInfo, context.CurrentPath));
            }
        }

        protected override IEnumerable<UIElement> GetItems(object context) =>
            _menuFactory.Create(_contextMenu, context);
    }
}