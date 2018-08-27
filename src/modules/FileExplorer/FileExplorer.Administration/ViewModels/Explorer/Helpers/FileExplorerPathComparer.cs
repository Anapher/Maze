using FileExplorer.Administration.Controls.Models;
using FileExplorer.Administration.Models;
using FileExplorer.Shared.Dtos;

namespace FileExplorer.Administration.ViewModels.Explorer.Helpers
{
    public class FileExplorerPathComparer : PathComparer, ICompareHierarchy<FileExplorerEntry>
    {
        private readonly IFileSystem _fileSystem;

        public FileExplorerPathComparer(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
            StringComparison = fileSystem.PathStringComparison;
        }

        public HierarchicalResult CompareHierarchy(FileExplorerEntry value1, FileExplorerEntry value2) =>
            CompareHierarchyInternal(_fileSystem.NormalizePath(value1?.Path), _fileSystem.NormalizePath(value2?.Path));

        public HierarchicalResult CompareHierarchy(string path1, string path2) =>
            CompareHierarchyInternal(_fileSystem.NormalizePath(path1), _fileSystem.NormalizePath(path2));
    }
}