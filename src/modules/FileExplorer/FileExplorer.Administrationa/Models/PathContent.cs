using System.Collections.Generic;
using FileExplorer.Shared.Dtos;

namespace FileExplorer.Administration.Models
{
    public class PathContent
    {
        public PathContent(DirectoryEntry directory, IReadOnlyList<FileExplorerEntry> entries,
            List<DirectoryEntry> pathDirectories)
        {
            Directory = directory;
            Entries = entries;
            PathDirectories = pathDirectories;
        }

        public DirectoryEntry Directory { get; }
        public string Path => Directory.Path;
        public IReadOnlyList<FileExplorerEntry> Entries { get; }
        public List<DirectoryEntry> PathDirectories { get; }
    }
}