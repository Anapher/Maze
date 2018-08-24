using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FileExplorer.Administration.Models.Cache;
using FileExplorer.Administration.Utilities;
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
        ///     Compare two paths on equality
        /// </summary>
        /// <param name="path1">The first path to compare</param>
        /// <param name="path2">The second path to compare</param>
        /// <returns>Return <code>true</code> if the paths are identical</returns>
        bool ComparePaths(string path1, string path2);

        /// <summary>
        ///     Unify a path so it can be used as case-sensitive key
        /// </summary>
        /// <param name="path">The path to unify</param>
        /// <returns>Return the unified path</returns>
        string UnifyPath(string path);

        /// <summary>
        ///     Normalize a path but keep casing
        /// </summary>
        /// <param name="path">The path to normalize</param>
        /// <returns>Return the normalized path</returns>
        string NormalizePath(string path);

        /// <summary>
        ///     Get the root elements. This must be called before <see cref="RequestPath"/> is used
        /// </summary>
        Task<RootElementsDto> GetRoot();

        /// <summary>
        ///     Request the content of a path
        /// </summary>
        /// <param name="path">The path to request</param>
        /// <param name="ignoreEntriesCache">True to ignore the cache for the <see cref="PathContent.Entries" /></param>
        /// <param name="ignorePathCache">True to ignore the cache of for the <see cref="PathContent.PathDirectories" /></param>
        /// <param name="token">The cancellation token for this operation</param>
        /// <returns>Return the content of the path</returns>
        Task<PathContent> RequestPath(string path, bool ignoreEntriesCache, bool ignorePathCache,
            CancellationToken token);

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
        CachedDirectory AddToCache(DirectoryEntry directoryEntry, IReadOnlyList<FileExplorerEntry> entries, bool directoriesOnly);
    }
}