using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using FileExplorer.Administration.Models.Args;
using FileExplorer.Administration.Models.Cache;
using FileExplorer.Shared.Dtos;

namespace FileExplorer.Administration.Models
{
    public interface IFileSystem
    {
        /// <summary>
        ///     Get the string comparison mode for paths
        /// </summary>
        StringComparison PathStringComparison { get; }

        /// <summary>
        ///     A hashset of illegal file name characters
        /// </summary>
        IImmutableSet<char> InvalidFileNameChars { get; }

        /// <summary>
        ///     Occurres when an entry was removed
        /// </summary>
        event EventHandler<FileExplorerEntry> EntryRemoved;

        /// <summary>
        ///     Occurres when an entry was updated (path changed)
        /// </summary>
        event EventHandler<EntryUpdatedEventArgs> EntryUpdated;

        /// <summary>
        ///     Occurres when an entry was added
        /// </summary>
        event EventHandler<FileExplorerEntry> EntryAdded;

        /// <summary>
        ///     Occurres when the entries of a directory were updated
        /// </summary>
        event EventHandler<DirectoryEntriesUpdatedEventArgs> DirectoryEntriesUpdated;

        /// <summary>
        ///     Check if the filename is valid
        /// </summary>
        /// <param name="filename">The filename to check</param>
        /// <returns>Return true if the filename is valid</returns>
        bool IsValidFilename(string filename);

        /// <summary>
        ///     Compare two paths on equality
        /// </summary>
        /// <param name="path1">The first path to compare</param>
        /// <param name="path2">The second path to compare</param>
        /// <returns>Return <code>true</code> if the paths are identical</returns>
        bool ComparePaths(string path1, string path2);

        /// <summary>
        ///     Normalize a path but keep casing
        /// </summary>
        /// <param name="path">The path to normalize</param>
        /// <returns>Return the normalized path</returns>
        string NormalizePath(string path);

        /// <summary>
        ///     Get the root elements. This must be called before <see cref="FetchPath" /> is used
        /// </summary>
        Task<RootElementsDto> GetRoot();

        /// <summary>
        ///     Fetch the content of a path
        /// </summary>
        /// <param name="path">The path to request</param>
        /// <param name="ignoreEntriesCache">True to ignore the cache for the <see cref="PathContent.Entries" /></param>
        /// <param name="ignorePathCache">True to ignore the cache of for the <see cref="PathContent.PathDirectories" /></param>
        /// <param name="token">The cancellation token for this operation</param>
        /// <returns>Return the content of the path</returns>
        Task<PathContent> FetchPath(string path, bool ignoreEntriesCache, bool ignorePathCache,
            CancellationToken token);

        /// <summary>
        ///     Fetch the sub directories of a directory
        /// </summary>
        /// <param name="directoryEntry">The directory which should be fetched</param>
        /// <param name="ignoreCache">True if the cache should be ignored</param>
        /// <returns>Return the fetched subdirectories of <see cref="directoryEntry" /></returns>
        Task<IEnumerable<DirectoryEntry>> FetchSubDirectories(DirectoryEntry directoryEntry, bool ignoreCache);

        /// <summary>
        ///     Get information about a file type
        /// </summary>
        /// <param name="filename">The path of the file</param>
        /// <returns>Information about the file type</returns>
        FileTypeDescription GetFileTypeDescription(string filename);

        /// <summary>
        /// </summary>
        /// <param name="labelPath"></param>
        /// <param name="labelId"></param>
        /// <returns></returns>
        string GetLabel(string labelPath, int labelId);

        /// <summary>
        ///     Add a directory to the cache
        /// </summary>
        /// <param name="directoryEntry">The directory entry</param>
        /// <param name="entries">The entries of the directory</param>
        /// <param name="directoriesOnly">True if only the directories were queried</param>
        /// <returns>Returns the created cache entry</returns>
        CachedDirectory AddToCache(DirectoryEntry directoryEntry, IReadOnlyList<FileExplorerEntry> entries,
            bool directoriesOnly);

        /// <summary>
        ///     Create a new directory
        /// </summary>
        /// <param name="path">The path of the directory to create</param>
        /// <returns></returns>
        Task CreateDirectory(string path);

        /// <summary>
        ///     Remove a file explorer entry
        /// </summary>
        /// <param name="entry">The entry to remove</param>
        Task Remove(FileExplorerEntry entry);

        /// <summary>
        ///     Move an entry to a new location
        /// </summary>
        /// <param name="entry">The entry that should be moved</param>
        /// <param name="path">The new path of the entry</param>
        Task Move(FileExplorerEntry entry, string path);

        /// <summary>
        ///     Adds the entry to the cache and notifies all subscribers
        /// </summary>
        /// <param name="entry">The entry that was added</param>
        void UploadCompleted(FileExplorerEntry entry);
    }
}