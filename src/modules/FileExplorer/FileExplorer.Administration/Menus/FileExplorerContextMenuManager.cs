using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using FileExplorer.Administration.Resources;
using FileExplorer.Administration.Utilities;
using FileExplorer.Administration.ViewModels;
using Orcus.Administration.Library.Menu;
using Orcus.Administration.Library.Menu.MenuBase;
using Prism.Commands;
using Unclassified.TxLib;

namespace FileExplorer.Administration.Menus
{
    public class FileExplorerContextMenuManager : ContextMenuManager
    {
        private readonly FileExplorerContextMenu _contextMenu;
        private readonly IImageProvider _imageProvider;
        private readonly IMenuFactory _menuFactory;
        private readonly VisualStudioIcons _visualStudioIcons;

        public FileExplorerContextMenuManager(FileExplorerContextMenu contextMenu, IMenuFactory menuFactory,
            VisualStudioIcons visualStudioIcons, IImageProvider imageProvider)
        {
            _contextMenu = contextMenu;
            _menuFactory = menuFactory;
            _visualStudioIcons = visualStudioIcons;
            _imageProvider = imageProvider;
        }

        public override void Build()
        {
            _contextMenu.Section1.Add(new ContextCommand<FileExplorerViewModel>
            {
                Header = Tx.T("FileExplorer:Refresh"),
                Icon = _visualStudioIcons.Refresh,
                Command = new DelegateCommand<FileExplorerViewModel>(x => RefreshDirectory(x))
            });

            _contextMenu.Section2.Add(new ContextCommand<FileExplorerViewModel>
            {
                Header = Tx.T("FileExplorer:UploadFile"),
                Icon = _visualStudioIcons.UploadFile,
                Command = new DelegateCommand<FileExplorerViewModel>(UploadFile)
            });
            _contextMenu.Section2.Add(new ContextCommand<FileExplorerViewModel>
            {
                Header = Tx.T("FileExplorer:DownloadFileToDirectory"),
                Icon = _visualStudioIcons.DownloadFile,
                Command = new DelegateCommand<FileExplorerViewModel>(DownloadFile)
            });

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

        private static Task RefreshDirectory(FileExplorerViewModel obj) => obj.OpenPath(obj.CurrentPath, true);

        private void CreateDirectory(FileExplorerViewModel obj)
        {
            throw new System.NotImplementedException();
        }

        private void DownloadFile(FileExplorerViewModel obj)
        {
            throw new System.NotImplementedException();
        }

        private void UploadFile(FileExplorerViewModel obj)
        {
            throw new System.NotImplementedException();
        }

        protected override IEnumerable<UIElement> GetItems(object context) =>
            _menuFactory.Create(_contextMenu, context);
    }
}