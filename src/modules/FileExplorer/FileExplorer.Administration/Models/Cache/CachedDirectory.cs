using System.Collections.Generic;
using System.Collections.Immutable;
using FileExplorer.Shared.Dtos;

namespace FileExplorer.Administration.Models.Cache
{
    public class CachedDirectory
    {
        public CachedDirectory(DirectoryEntry directory, bool directoriesOnly, IEnumerable<FileExplorerEntry> entries)
        {
            Directory = directory;
            DirectoriesOnly = directoriesOnly;
            Entries = entries.ToImmutableList();
        }

        public DirectoryEntry Directory { get; }
        public bool DirectoriesOnly { get; }
        public IImmutableList<FileExplorerEntry> Entries { get; set; }
        public object EntriesLock { get; } = new object();
    }
}