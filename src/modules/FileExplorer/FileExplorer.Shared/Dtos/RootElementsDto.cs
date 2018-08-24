using System.Collections.Generic;

namespace FileExplorer.Shared.Dtos
{
    public class RootElementsDto
    {
        public List<DirectoryEntry> RootDirectories { get; set; }
        public DirectoryEntry ComputerDirectory { get; set; }
        public List<FileExplorerEntry> ComputerDirectoryEntries { get; set; }
    }
}