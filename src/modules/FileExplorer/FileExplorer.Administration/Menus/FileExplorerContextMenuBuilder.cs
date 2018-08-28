using System.Threading.Tasks;
using FileExplorer.Administration.ViewModels;
using Orcus.Administration.Library.Menu.MenuBase;
using Prism.Commands;

namespace FileExplorer.Administration.Menus
{
    public class FileExplorerContextMenuBuilder
    {
        public void Build(FileExplorerContextMenu contextMenu)
        {
            contextMenu.Section1.Add(new ContextCommand<FileExplorerViewModel>
            {
                Header = "Refresh",
                Command = new DelegateCommand<FileExplorerViewModel>(x => Refresh(x))
            });
        }

        private static Task Refresh(FileExplorerViewModel obj) => obj.OpenPath(obj.CurrentPath, true);
    }
}