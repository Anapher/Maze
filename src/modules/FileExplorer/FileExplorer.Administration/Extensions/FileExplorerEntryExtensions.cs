using FileExplorer.Shared.Dtos;

namespace FileExplorer.Administration.Extensions
{
    public static class FileExplorerEntryExtensions
    {
        public static T Migrate<T>(this T entry, DirectoryEntry parentEntry) where T : FileExplorerEntry
        {
            entry.Parent = parentEntry;
            return entry;
        }

        public static T Migrate<T>(this T entry) where T : FileExplorerEntry => entry.Migrate(null);
    }
}