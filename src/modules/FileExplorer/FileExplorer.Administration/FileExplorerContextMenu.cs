using FileExplorer.Administration.ViewModels;
using FileExplorer.Administration.ViewModels.Explorer;
using FileExplorer.Administration.ViewModels.Explorer.Base;
using Maze.Administration.Library.Menu.MenuBase;

namespace FileExplorer.Administration
{
    public class FileExplorerContextMenu : MenuSection<ContextCommand<FileExplorerViewModel>>
    {
        public FileExplorerContextMenu()
        {
            Add(Section1 = new MenuSection<ContextCommand<FileExplorerViewModel>>());
            Add(Section2 = new MenuSection<ContextCommand<FileExplorerViewModel>>());
            Add(Section3New = new NavigationalEntry<ContextCommand<FileExplorerViewModel>>());
        }

        public MenuSection<ContextCommand<FileExplorerViewModel>> Section1 { get; }
        public MenuSection<ContextCommand<FileExplorerViewModel>> Section2 { get; }
        public NavigationalEntry<ContextCommand<FileExplorerViewModel>> Section3New { get; }
    }

    public class FileExplorerListDirectoryContextMenu : MenuSection<ContextDiItemsCommand<DirectoryViewModel, EntryViewModel, FileExplorerViewModel>>
    {
        public FileExplorerListDirectoryContextMenu()
        {
            Add(Section1 = new MenuSection<ContextDiItemsCommand<DirectoryViewModel, EntryViewModel, FileExplorerViewModel>>());
            Section1.Add(Section1Path = new NavigationalEntry<ContextDiItemsCommand<DirectoryViewModel, EntryViewModel, FileExplorerViewModel>>());
            Add(Section2 = new MenuSection<ContextDiItemsCommand<DirectoryViewModel, EntryViewModel, FileExplorerViewModel>>());
            Section2.Add(Section2Archive = new NavigationalEntry<ContextDiItemsCommand<DirectoryViewModel, EntryViewModel, FileExplorerViewModel>>());
            Add(Section3 = new MenuSection<ContextDiItemsCommand<DirectoryViewModel, EntryViewModel, FileExplorerViewModel>>());
            Add(Section4 = new MenuSection<ContextDiItemsCommand<DirectoryViewModel, EntryViewModel, FileExplorerViewModel>>());
        }

        public MenuSection<ContextDiItemsCommand<DirectoryViewModel, EntryViewModel, FileExplorerViewModel>> Section1 { get; }
        public NavigationalEntry<ContextDiItemsCommand<DirectoryViewModel, EntryViewModel, FileExplorerViewModel>> Section1Path { get; }
        public MenuSection<ContextDiItemsCommand<DirectoryViewModel, EntryViewModel, FileExplorerViewModel>> Section2 { get; }
        public NavigationalEntry<ContextDiItemsCommand<DirectoryViewModel, EntryViewModel, FileExplorerViewModel>> Section2Archive { get; }
        public MenuSection<ContextDiItemsCommand<DirectoryViewModel, EntryViewModel, FileExplorerViewModel>> Section3 { get; }
        public MenuSection<ContextDiItemsCommand<DirectoryViewModel, EntryViewModel, FileExplorerViewModel>> Section4 { get; }
    }

    public class FileExplorerListFileContextMenu : MenuSection<ContextDiItemsCommand<FileViewModel, EntryViewModel, FileExplorerViewModel>>
    {
        public FileExplorerListFileContextMenu()
        {
            Add(Section1 = new MenuSection<ContextDiItemsCommand<FileViewModel, EntryViewModel, FileExplorerViewModel>>());
            Add(Section2 = new MenuSection<ContextDiItemsCommand<FileViewModel, EntryViewModel, FileExplorerViewModel>>());
            Section2.Add(Section2Path = new NavigationalEntry<ContextDiItemsCommand<FileViewModel, EntryViewModel, FileExplorerViewModel>>());
            Add(Section3 = new MenuSection<ContextDiItemsCommand<FileViewModel, EntryViewModel, FileExplorerViewModel>>());
            Section3.Add(Section3Archive = new NavigationalEntry<ContextDiItemsCommand<FileViewModel, EntryViewModel, FileExplorerViewModel>>());
            Add(Section4 = new MenuSection<ContextDiItemsCommand<FileViewModel, EntryViewModel, FileExplorerViewModel>>());
            Add(Section5 = new MenuSection<ContextDiItemsCommand<FileViewModel, EntryViewModel, FileExplorerViewModel>>());
        }

        public MenuSection<ContextDiItemsCommand<FileViewModel, EntryViewModel, FileExplorerViewModel>> Section1 { get; }
        public MenuSection<ContextDiItemsCommand<FileViewModel, EntryViewModel, FileExplorerViewModel>> Section2 { get; }
        public NavigationalEntry<ContextDiItemsCommand<FileViewModel, EntryViewModel, FileExplorerViewModel>> Section2Path { get; }
        public MenuSection<ContextDiItemsCommand<FileViewModel, EntryViewModel, FileExplorerViewModel>> Section3 { get; }
        public NavigationalEntry<ContextDiItemsCommand<FileViewModel, EntryViewModel, FileExplorerViewModel>> Section3Archive { get; }
        public MenuSection<ContextDiItemsCommand<FileViewModel, EntryViewModel, FileExplorerViewModel>> Section4 { get; }
        public MenuSection<ContextDiItemsCommand<FileViewModel, EntryViewModel, FileExplorerViewModel>> Section5 { get; }
    }
}