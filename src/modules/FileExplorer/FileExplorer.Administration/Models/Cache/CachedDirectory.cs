using System.Collections.Generic;
using FileExplorer.Shared.Dtos;

namespace FileExplorer.Administration.Models.Cache
{
    public class CachedDirectory
    {
        public CachedDirectory(DirectoryEntry directory, bool directoriesOnly, IReadOnlyList<FileExplorerEntry> entries)
        {
            Directory = directory;
            DirectoriesOnly = directoriesOnly;
            Entries = entries;
        }

        public DirectoryEntry Directory { get; }
        public bool DirectoriesOnly { get; }
        public IReadOnlyList<FileExplorerEntry> Entries { get; }
    }
}