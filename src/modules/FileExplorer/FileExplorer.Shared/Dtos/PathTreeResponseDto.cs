using System.Collections.Generic;

namespace FileExplorer.Shared.Dtos
{
    public class PathTreeResponseDto
    {
        public IDictionary<int, List<DirectoryEntry>> Directories { get; set; }
        public List<FileExplorerEntry> Entries { get; set; }
    }
}