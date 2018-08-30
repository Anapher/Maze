using System.IO;
using System.Threading.Tasks;
using FileExplorer.Shared.Dtos;

namespace FileExplorer.Administration.Models
{
    public static class FileSystemExtensions
    {
        public static Task Rename(this IFileSystem fileSystem, FileExplorerEntry entry, string newName)
        {
            var directory = Path.GetDirectoryName(entry.Path);
            return fileSystem.Move(entry, Path.Combine(directory, newName));
        }
    }
}