using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using FileExplorer.Client.Utilities;
using FileExplorer.Shared.Dtos;

namespace FileExplorer.Client.Extensions
{
    public static class DirectoryHelperExtensions
    {
        public static async Task<IEnumerable<DirectoryEntry>> GetDirectoryEntries(this DirectoryHelper directoryHelper,
            string directoryPath, CancellationToken token)
        {
            using (var directory = new DirectoryInfoEx(directoryPath))
            {
                return await directoryHelper.GetDirectoryEntries(directory, token);
            }
        }

        public static async Task<IEnumerable<FileExplorerEntry>> GetEntries(this DirectoryHelper directoryHelper,
            string directoryPath, CancellationToken token)
        {
            using (var directory = new DirectoryInfoEx(directoryPath))
            {
                return await directoryHelper.GetEntries(directory, token);
            }
        }

        public static async Task<IEnumerable<FileExplorerEntry>> GetEntriesKeepOrder(this DirectoryHelper directoryHelper,
            string directoryPath, CancellationToken token)
        {
            using (var directory = new DirectoryInfoEx(directoryPath))
            {
                return await directoryHelper.GetEntriesKeepOrder(directory, token);
            }
        }
    }
}