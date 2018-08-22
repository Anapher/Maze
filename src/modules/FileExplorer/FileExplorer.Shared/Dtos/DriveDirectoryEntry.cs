using System.IO;

namespace FileExplorer.Shared.Dtos
{
    public class DriveDirectoryEntry : SpecialDirectoryEntry
    {
        public long TotalSize { get; set; }
        public long UsedSpace { get; set; }
        public DriveType DriveType { get; set; }

        public override FileExplorerEntryType Type { get; } = FileExplorerEntryType.Drive;
    }
}