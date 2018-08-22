namespace FileExplorer.Shared.Dtos
{
    public class DirectoryEntry : FileExplorerEntry
    {
        public bool HasSubFolder { get; set; }

        public override FileExplorerEntryType Type { get; } = FileExplorerEntryType.Directory;
    }
}