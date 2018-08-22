namespace FileExplorer.Shared.Dtos
{
    public class FileEntry : FileExplorerEntry
    {
        public long Size { get; set; }

        public override FileExplorerEntryType Type { get; } = FileExplorerEntryType.File;
    }
}