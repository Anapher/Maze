using System.Linq;
using System.Threading.Tasks;
using FileExplorer.Administration.Helpers;
using FileExplorer.Administration.Models;
using FileExplorer.Administration.ViewModels.Explorer;
using FileExplorer.Administration.ViewModels.Explorer.Helpers;
using FileExplorer.Shared.Dtos;
using Prism.Mvvm;

namespace FileExplorer.Administration.ViewModels
{
    public class DirectoryTreeViewModel : BindableBase, ISupportTreeSelector<DirectoryNodeViewModel, FileExplorerEntry>
    {
        private readonly IFileSystem _fileSystem;

        public DirectoryTreeViewModel(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;

            Entries = new EntriesHelper<DirectoryNodeViewModel>();
            Selection = new TreeRootSelector<DirectoryNodeViewModel, FileExplorerEntry>(Entries){Comparers = new []
            {
                new FileExplorerPathComparer(fileSystem)
            }};
        }

        public IEntriesHelper<DirectoryNodeViewModel> Entries { get; set; }
        public ITreeSelector<DirectoryNodeViewModel, FileExplorerEntry> Selection { get; set; }

        public async Task SelectAsync(FileExplorerEntry value)
        {
            await Selection.LookupAsync(value,
                RecrusiveSearch<DirectoryNodeViewModel, FileExplorerEntry>.LoadSubentriesIfNotLoaded,
                SetSelected<DirectoryNodeViewModel, FileExplorerEntry>.WhenSelected,
                SetExpanded<DirectoryNodeViewModel, FileExplorerEntry>.WhenChildSelected);
        }

        public void InitializeRootElements(RootElementsDto dto)
        {
            var rootElements = dto.RootDirectories.ToList();
            rootElements.Add(dto.ComputerDirectory);
            
        }
    }
}