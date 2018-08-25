using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FileExplorer.Administration.Cache;
using FileExplorer.Administration.Extensions;
using FileExplorer.Administration.Models.Cache;
using FileExplorer.Administration.Rest;
using FileExplorer.Administration.Utilities;
using FileExplorer.Shared.Dtos;
using Microsoft.Extensions.Caching.Memory;
using Orcus.Administration.Library.Clients;
using Orcus.Administration.Library.Extensions;

namespace FileExplorer.Administration.Models
{
    public class RemoteFileSystem : IFileSystem
    {
        private readonly IMemoryCache _globalCache;
        private readonly IPackageRestClient _restClient;
        private readonly IMemoryCache _localCache;
        private readonly bool _caseInsensitivePaths = true;
        private CachedDirectory _computerDirectory;

        public RemoteFileSystem(IMemoryCache globalCache, IPackageRestClient restClient)
        {
            _globalCache = globalCache;
            _restClient = restClient;
            _localCache = new MemoryCache(new MemoryCacheOptions());
        }

        public StringComparison PathStringComparison { get; } = StringComparison.OrdinalIgnoreCase;

        public bool ComparePaths(string path1, string path2)
        {
            return string.Equals(NormalizePath(path1), NormalizePath(path2), PathStringComparison);
        }

        public string UnifyPath(string path)
        {
            path = NormalizePath(path);
            if (_caseInsensitivePaths)
                path = path.ToLower();

            return path;
        }

        public string NormalizePath(string path)
        {
            if (string.IsNullOrEmpty(path))
                return null;

            path = Path.GetFullPath(path).TrimEnd('\\');

            //Volume Label, we add a slash because some systems (Windows XP) can't handle "C:"
            if (path.Length == 2 && path[1] == ':')
                return path + "\\";

            return path;
        }

        public async Task<RootElementsDto> GetRoot()
        {
            var dto = await FileExplorerResource.GetRoot(_restClient);

            foreach (var rootDirectory in dto.RootDirectories)
                rootDirectory.Migrate();

            foreach (var computerDirectoryEntry in dto.ComputerDirectoryEntries)
                computerDirectoryEntry.Migrate(dto.ComputerDirectory);

            _computerDirectory = AddToCache(dto.ComputerDirectory, dto.ComputerDirectoryEntries, false);
            return dto;
        }

        public async Task<PathContent> FetchPath(string path, bool ignoreEntriesCache, bool ignorePathCache,
            CancellationToken token)
        {
            if (PathHelper.ContainsEnvironmentVariables(path))
            {
                path = await FileSystemResource.ExpandEnvironmentVariables(path, _restClient);
            }

            path = NormalizePath(path);
            var request = new PathTreeRequestDto {Path = path, RequestedDirectories = new List<int>()};

            if (ignoreEntriesCache || !TryGetCachedDirectory(path, out var cachedDirectory) ||
                cachedDirectory.DirectoriesOnly)
            {
                request.RequestEntries = true;
            }

            var index = 0;
            var parts = new List<string>();

            //forward loop, e. g.
            //C:\
            //C:\Windows
            //C:\Windows\System32 ...
            while (true)
            {
                index = path.IndexOf('\\', index);
                if (index == -1)
                    break;

                var partPath = path.Substring(0, index);

                if (ignorePathCache || !TryGetCachedDirectory(partPath, out _))
                    request.RequestedDirectories.Add(parts.Count);

                parts.Add(partPath);
            }

            PathTreeResponseDto queryResponse = null;
            if (request.RequestEntries || request.RequestedDirectories.Any())
            {
                queryResponse = await FileExplorerResource.GetPathTree(request, _restClient);
            }

            parts.Add(path);

            DirectoryEntry directory = null;
            var pathDirectories = new List<DirectoryEntry>();
            IReadOnlyList<FileExplorerEntry> directoryEntries = null;

            for (var i = 0; i < parts.Count; i++)
            {
                var directoryPath = parts[i];

                if (directory == null)
                {
                    if (TryGetCachedDirectory(directoryPath, out cachedDirectory))
                        directory = cachedDirectory.Directory;
                    else
                    {
                        directory = _computerDirectory.Entries.OfType<DirectoryEntry>()
                            .FirstOrDefault(x => string.Equals(x.Path, directoryPath, PathStringComparison));
                    }
                }

                if (directory == null) //Special folders like trash can etc.
                {
                    directory = await FileSystemResource.GetDirectoryEntry(directoryPath, _restClient);
                    directory.Migrate();
                }

                if (queryResponse != null && queryResponse.Directories.TryGetValue(i, out var subDirectories))
                {
                    directoryEntries = subDirectories;

                    foreach (var directoryEntry in directoryEntries)
                        directoryEntry.Migrate(directory);

                    AddToCache(directory, directoryEntries, true);
                }
                else
                {
                    directoryEntries = _localCache
                        .Get<CachedDirectory>(new CachedDirectoryKey {UnifiedPath = UnifyPath(directoryPath)}).Entries;
                }

                pathDirectories.Add(directory);

                //if we are not at the very last part (so there is a child directory)
                if (i > parts.Count - 1)
                {
                    var normalizedPath = NormalizePath(parts[i + 1]); //path of the child directory
                    var nextDirectory = directoryEntries.OfType<DirectoryEntry>().FirstOrDefault(x =>
                        string.Equals(normalizedPath, NormalizePath(x.Path), PathStringComparison));

                    if (nextDirectory == null)
                    {
                        nextDirectory = new DirectoryEntry
                        {
                            HasSubFolder = true,
                            Parent = directory,
                            Name = GetDirectoryName(normalizedPath)
                        };
                        directoryEntries = directoryEntries.Concat(new[] {nextDirectory}).ToList();

                        //update cache
                        var cacheKey = new CachedDirectoryKey {UnifiedPath = UnifyPath(directoryPath)};
                        var oldCache = _localCache.Get<CachedDirectory>(cacheKey);
                        _localCache.CreateEntry(cacheKey).SetValue(new CachedDirectory(oldCache.Directory,
                            oldCache.DirectoriesOnly, directoryEntries));
                    }

                    directory = nextDirectory;
                }
            }

            return new PathContent(directory, directoryEntries, pathDirectories);
        }

        public async Task<IEnumerable<DirectoryEntry>> FetchSubDirectories(DirectoryEntry directoryEntry, bool ignoreCache)
        {
            if (!ignoreCache && TryGetCachedDirectory(directoryEntry.Path, out var cachedDirectory))
            {
                return cachedDirectory.Entries.OfType<DirectoryEntry>();
            }

            var directories = await FileSystemResource.QueryDirectories(directoryEntry.Path, _restClient);
            foreach (var subDirectory in directories)
                subDirectory.Migrate(directoryEntry);

            AddToCache(directoryEntry, directories, true);
            return directories;
        }

        public FileTypeDescription GetFileTypeDescription(string filename)
        {
            var extension = Path.GetExtension(filename);
            if (string.IsNullOrEmpty(extension))
                return null;

            var cacheKey = new FileDescriptionKey {Extension = extension.ToUpper()};
            return _globalCache.GetOrSetValueSafe(cacheKey, TimeSpan.FromMinutes(10),
                () => FileHelper.GetFileTypeDescription(filename));
        }

        public string GetLabel(string labelPath, int labelId)
        {
            labelPath = NormalizePath(Environment.ExpandEnvironmentVariables(labelPath)).ToLowerInvariant(); //we are on Windows here
            var cacheKey = new LoadedLabelLibraryKey {Path = labelPath};

            var library = _globalCache.GetOrSetValueSafe(cacheKey, entry =>
            {
                entry.SetSlidingExpiration(TimeSpan.FromMinutes(5));
                entry.RegisterPostEvictionCallback((key, o, reason, state) =>
                {
                    var val = (Lazy<LoadedLabelLibrary>) o;
                    if (val.IsValueCreated) val.Value.Dispose();
                });
            }, () => DirectoryHelper.GetLabelLibrary(labelPath));

            return library.LoadString(labelId);
        }

        private string GetDirectoryName(string path)
        {
            try
            {
                return Path.GetFileName(path);
            }
            catch (Exception e)
            {
                Debug.Fail($"Why does that happen? {e.Message} / {path}");
                var pos = path.LastIndexOf('\\');
                if (pos == -1)
                    return path;
                return path.Substring(pos + 1);
            }
        }

        private bool TryGetCachedDirectory(string path, out CachedDirectory cachedDirectory)
        {
            return _localCache.TryGetValue(new CachedDirectoryKey {UnifiedPath = UnifyPath(path)}, out cachedDirectory);
        }

        public CachedDirectory AddToCache(DirectoryEntry directory, IReadOnlyList<FileExplorerEntry> entries, bool directoriesOnly)
        {
            var cachedDirectory = new CachedDirectory(directory, directoriesOnly, entries);
            var cacheKey = new CachedDirectoryKey {UnifiedPath = UnifyPath(directory.Path)};

            _localCache.CreateEntry(cacheKey).SetValue(cachedDirectory);
            return cachedDirectory;
        }
    }
}