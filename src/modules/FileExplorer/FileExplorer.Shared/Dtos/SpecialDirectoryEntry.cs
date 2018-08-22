namespace FileExplorer.Shared.Dtos
{
    public class SpecialDirectoryEntry : DirectoryEntry
    {
        public string Label { get; set; }
        public int LabelId { get; set; }
        public string LabelPath { get; set; }
        public int IconId { get; set; }

        public override FileExplorerEntryType Type { get; } = FileExplorerEntryType.SpecialDirectory;
    }
}