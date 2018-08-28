using FileExplorer.Administration.ViewModels;
using FileExplorer.Administration.ViewModels.Explorer;
using Orcus.Administration.Library.Menu.MenuBase;

namespace FileExplorer.Administration
{
    public class FileExplorerContextMenu : MenuSection<ContextCommand<FileExplorerViewModel>>
    {
        public FileExplorerContextMenu()
        {
            Add(Section1 = new MenuSection<ContextCommand<FileExplorerViewModel>>());
            Add(Section2 = new MenuSection<ContextCommand<FileExplorerViewModel>>());
            Add(Section3New = new MenuSection<ContextCommand<FileExplorerViewModel>>());
        }

        public MenuSection<ContextCommand<FileExplorerViewModel>> Section1 { get; }
        public MenuSection<ContextCommand<FileExplorerViewModel>> Section2 { get; }
        public MenuSection<ContextCommand<FileExplorerViewModel>> Section3New { get; }
    }

    public class FileExplorerListDirectoryContextMenu : MenuSection<ContextItemCommand<DirectoryNodeViewModel, FileExplorerViewModel>>
    {
        public FileExplorerListDirectoryContextMenu()
        {
            Add(Section1 = new MenuSection<ContextItemCommand<DirectoryNodeViewModel, FileExplorerViewModel>>());
            Add(Section2 = new MenuSection<ContextItemCommand<DirectoryNodeViewModel, FileExplorerViewModel>>());
            Add(Section3 = new MenuSection<ContextItemCommand<DirectoryNodeViewModel, FileExplorerViewModel>>());
        }

        public MenuSection<ContextItemCommand<DirectoryNodeViewModel, FileExplorerViewModel>> Section1 { get; }
        public MenuSection<ContextItemCommand<DirectoryNodeViewModel, FileExplorerViewModel>> Section2 { get; }
        public MenuSection<ContextItemCommand<DirectoryNodeViewModel, FileExplorerViewModel>> Section3 { get; }
    }

    public class FileExplorerListFileContextMenu : MenuSection<ContextItemCommand<FileViewModel, FileExplorerViewModel>>
    {
        public FileExplorerListFileContextMenu()
        {
            Add(Section1 = new MenuSection<ContextItemCommand<FileViewModel, FileExplorerViewModel>>());
            Add(Section2 = new MenuSection<ContextItemCommand<FileViewModel, FileExplorerViewModel>>());
            Add(Section3 = new MenuSection<ContextItemCommand<FileViewModel, FileExplorerViewModel>>());
            Add(Section4 = new MenuSection<ContextItemCommand<FileViewModel, FileExplorerViewModel>>());
        }

        public MenuSection<ContextItemCommand<FileViewModel, FileExplorerViewModel>> Section1 { get; }
        public MenuSection<ContextItemCommand<FileViewModel, FileExplorerViewModel>> Section2 { get; }
        public MenuSection<ContextItemCommand<FileViewModel, FileExplorerViewModel>> Section3 { get; }
        public MenuSection<ContextItemCommand<FileViewModel, FileExplorerViewModel>> Section4 { get; }
    }
}