namespace FileExplorer.Shared.Dtos
{
    public class DirectoryEntry : FileExplorerEntry
    {
        public string Label { get; set; }
        public bool HasSubFolder { get; set; }

        public override FileExplorerEntryType Type { get; } = FileExplorerEntryType.Directory;
    }
}