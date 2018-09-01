using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FileExplorer.Administration.Cache;
using FileExplorer.Administration.Extensions;
using FileExplorer.Administration.Models.Args;
using FileExplorer.Administration.Models.Cache;
using FileExplorer.Administration.Rest;
using FileExplorer.Administration.Utilities;
using FileExplorer.Shared.Dtos;
using Microsoft.Extensions.Caching.Memory;
using Orcus.Administration.Library.Clients;
using Orcus.Administration.Library.Extensions;
using FileExplorer.Shared.Utilities;

namespace FileExplorer.Administration.Models
{
    public class RemoteFileSystem : IFileSystem
    {
        private readonly IMemoryCache _globalCache;
        private readonly IPackageRestClient _restClient;
        private readonly ConcurrentDictionary<string, CachedDirectory> _localCache;
        private readonly bool _caseInsensitivePaths = true;
        private CachedDirectory _computerDirectory;

        public RemoteFileSystem(IMemoryCache globalCache, IPackageRestClient restClient)
        {
            _globalCache = globalCache;
            _restClient = restClient;
            _localCache = new ConcurrentDictionary<string, CachedDirectory>(_caseInsensitivePaths
                ? StringComparer.OrdinalIgnoreCase
                : StringComparer.Ordinal);

            InvalidFileNameChars = Path.GetInvalidFileNameChars().ToImmutableHashSet();
        }

        public IImmutableSet<char> InvalidFileNameChars { get; }
        public StringComparison PathStringComparison { get; } = StringComparison.OrdinalIgnoreCase;

        public event EventHandler<FileExplorerEntry> EntryRemoved;
        public event EventHandler<EntryUpdatedEventArgs> EntryUpdated;
        public event EventHandler<FileExplorerEntry> EntryAdded;
        public event EventHandler<DirectoryEntriesUpdatedEventArgs> DirectoryEntriesUpdated;

        public bool IsValidFilename(string filename) =>
            !string.IsNullOrWhiteSpace(filename) && !filename.Any(x => InvalidFileNameChars.Contains(x));

        public bool ComparePaths(string path1, string path2)
        {
            return string.Equals(NormalizePath(path1), NormalizePath(path2), PathStringComparison);
        }

        public string NormalizePath(string path)
        {
            if (string.IsNullOrEmpty(path))
                return null;

            if (!path.Any(x => InvalidFileNameChars.Contains(x)))
                path = Path.GetFullPath(path);

            path = path.TrimEnd('\\');

            //Volume Label, we add a slash because some systems (Windows XP) can't handle "C:"
            if (path.Length == 2 && path[1] == ':')
                return path + "\\";

            return path;
        }

        public async Task<RootElementsDto> GetRoot()
        {
            var dto = await FileExplorerResource.GetRoot(_restClient);
            dto.ComputerDirectory.Migrate();

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

            var parts = PathHelper.GetPathDirectories(path).ToList();
            for (var i = 0; i < parts.Count; i++)
            {
                var partPath = parts[i];

                if (ignorePathCache || !TryGetCachedDirectory(partPath, out _))
                    request.RequestedDirectories.Add(i);
            }

            PathTreeResponseDto queryResponse = null;
            if (request.RequestEntries || request.RequestedDirectories.Any())
            {
                queryResponse = await FileExplorerResource.GetPathTree(request,
                    DirectoryHelper.IsComputerDirectory(path), token, _restClient);
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

                if (request.RequestEntries && i == parts.Count - 1)
                {
                    // ReSharper disable once PossibleNullReferenceException
                    directoryEntries = queryResponse.Entries;
                    foreach (var entry in directoryEntries)
                        entry.Migrate(directory);

                    AddToCache(directory, directoryEntries, false);
                }
                else if (queryResponse?.Directories != null && queryResponse.Directories.TryGetValue(i, out var subDirectories))
                {
                    directoryEntries = subDirectories;

                    foreach (var directoryEntry in directoryEntries)
                        directoryEntry.Migrate(directory);

                    AddToCache(directory, directoryEntries, true);
                }
                else
                {
                    directoryEntries = _localCache[NormalizePath(directoryPath)].Entries.ToList();
                }

                pathDirectories.Add(directory);

                //if we are not at the very last part (so there is a child directory)
                if (i < parts.Count - 1)
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
                        var key = NormalizePath(directoryPath);
                        var oldCache = _localCache[key];
                        _localCache[key] = new CachedDirectory(oldCache.Directory, oldCache.DirectoriesOnly,
                            directoryEntries);
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
            return _localCache.TryGetValue(NormalizePath(path), out cachedDirectory);
        }

        public CachedDirectory AddToCache(DirectoryEntry directory, IReadOnlyList<FileExplorerEntry> entries, bool directoriesOnly)
        {
            var cachedDirectory = new CachedDirectory(directory, directoriesOnly, entries);
            var cacheKey = NormalizePath(directory.Path);
            
            _localCache[cacheKey] = cachedDirectory;
            DirectoryEntriesUpdated?.Invoke(this,
                new DirectoryEntriesUpdatedEventArgs(directory.Path, cachedDirectory.Entries, directoriesOnly));
            return cachedDirectory;
        }

        public async Task CreateDirectory(string path)
        {
            await FileSystemResource.CreateDirectory(path, _restClient);

            var entry = new DirectoryEntry
            {
                CreationTime = DateTimeOffset.Now,
                HasSubFolder = false,
                Name = Path.GetFileName(path),
                Path = path
            };
            UploadCompleted(entry);
        }

        public void UploadCompleted(FileExplorerEntry entry)
        {
            var parentFolderPath = Path.GetDirectoryName(entry.Path);

            if (parentFolderPath != null && TryGetCachedDirectory(parentFolderPath, out var cachedDirectory))
            {
                entry.Parent = cachedDirectory.Directory;
                cachedDirectory.Directory.HasSubFolder = true;
                lock (cachedDirectory.EntriesLock)
                {
                    cachedDirectory.Entries = cachedDirectory.Entries.Add(entry);
                }
            }

            EntryAdded?.Invoke(this, entry);
        }

        public async Task Remove(FileExplorerEntry entry)
        {
            if (entry.Type == FileExplorerEntryType.File)
            {
                await FileSystemResource.DeleteFile(entry.Path, _restClient);
            }
            else
            {
                await FileSystemResource.DeleteDirectory(entry.Path, _restClient);
            }

            EntryRemoved?.Invoke(this, entry);

            var parentFolder = Path.GetDirectoryName(entry.Path);
            if (parentFolder != null && TryGetCachedDirectory(parentFolder, out var cachedDirectory))
            {
                lock (cachedDirectory.EntriesLock)
                {
                    cachedDirectory.Entries = cachedDirectory.Entries.Remove(entry);
                }
            }

            if (entry.Type != FileExplorerEntryType.File)
            {
                var normalizedPath = NormalizePath(entry.Path);
                foreach (var keyValuePair in _localCache.Where(x =>
                    x.Key.StartsWith(normalizedPath, PathStringComparison)))
                    _localCache.TryRemove(keyValuePair.Key, out _);
            }
        }

        public async Task Move(FileExplorerEntry entry, string path)
        {
            path = NormalizePath(path);

            if (entry.Type == FileExplorerEntryType.File)
                await FileSystemResource.MoveFile(entry.Path, path, _restClient);
            else
                await FileSystemResource.MoveDirectory(entry.Path, path, _restClient);

            var oldPath = NormalizePath(entry.Path);
            entry.Path = path;
            entry.Name = Path.GetFileName(path);

            EntryUpdated?.Invoke(this, new EntryUpdatedEventArgs(entry, oldPath));

            if (entry.Type != FileExplorerEntryType.File)
            {
                foreach (var cachedEntry in _localCache.Where(x => x.Key.StartsWith(oldPath, PathStringComparison)))
                {
                    _localCache.TryRemove(cachedEntry.Key, out _);
                    var newPath = path + cachedEntry.Key.Substring(oldPath.Length);
                    cachedEntry.Value.Directory.Path = newPath;

                    lock (cachedEntry.Value.EntriesLock)
                    {
                        foreach (var fileExplorerEntry in cachedEntry.Value.Entries)
                            fileExplorerEntry.Path = path + fileExplorerEntry.Path.Substring(oldPath.Length);
                    }

                    _localCache[newPath] = cachedEntry.Value;
                }
            }
        }
    }
}