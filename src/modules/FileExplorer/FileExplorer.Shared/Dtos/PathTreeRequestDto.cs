using System.Collections.Generic;

namespace FileExplorer.Shared.Dtos
{
    public class PathTreeRequestDto
    {
        public string Path { get; set; }
        public List<int> RequestedDirectories { get; set; }
        public bool RequestEntries { get; set; }
    }
}