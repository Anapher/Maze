using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FileExplorer.Client.Extensions;
using FileExplorer.Client.Native;
using FileExplorer.Shared.Dtos;
using Microsoft.Win32;
using Maze.Client.Library.Utilities;
using Maze.Utilities;
using Serilog;
using ShellDll;

namespace FileExplorer.Client.Utilities
{
    public class DirectoryHelper
    {
        private readonly Lazy<DriveInfo[]> _drives;
        private static readonly ILogger Logger = Log.ForContext<DirectoryHelper>();

        public DirectoryHelper()
        {
            _drives = new Lazy<DriveInfo[]>(DriveInfo.GetDrives, LazyThreadSafetyMode.ExecutionAndPublication);
        }

        public async Task<IEnumerable<FileExplorerEntry>> GetEntriesKeepOrder(DirectoryInfoEx directory, CancellationToken token)
        {
            var entries = directory.EnumerateFileSystemInfos("*", SearchOption.TopDirectoryOnly).ToList();
            var result = new FileExplorerEntry[entries.Count];

            await TaskCombinators.ThrottledAsync(entries, (entry, _) => Task.Run(() =>
            {
                var index = entries.IndexOf(entry);
                using (entry)
                {
                    if (entry.IsFolder)
                        result[index] = GetDirectoryEntry((DirectoryInfoEx) entry, directory);
                    else
                        result[index] = GetFileEntry((FileInfoEx) entry);
                }
            }), token);

            return result;
        }

        public Task<IEnumerable<FileExplorerEntry>> GetEntries(DirectoryInfoEx directory, CancellationToken token)
        {
            var entries = directory.EnumerateFileSystemInfos("*", SearchOption.TopDirectoryOnly,
                () => token.IsCancellationRequested);
            return TaskCombinators.ThrottledAsync(entries, (entry, _) => Task.Run(() =>
            {
                using (entry)
                {
                    if (entry.IsFolder)
                        return GetDirectoryEntry((DirectoryInfoEx) entry, directory);

                    return GetFileEntry((FileInfoEx) entry);
                }
            }, token), CancellationToken.None);
        }

        public Task<IEnumerable<DirectoryEntry>> GetDirectoryEntries(DirectoryInfoEx directory, CancellationToken token)
        {
            var entries = directory.EnumerateDirectories("*", SearchOption.TopDirectoryOnly,
                () => token.IsCancellationRequested);
            return TaskCombinators.ThrottledAsync(entries, (entry, _) => Task.Run(() =>
            {
                using (entry)
                    return GetDirectoryEntry(entry, directory);
            }), token);
        }

        public async Task<List<FileExplorerEntry>> GetComputerDirectoryEntries()
        {
            var entries = (await GetEntries(DirectoryInfoEx.MyComputerDirectory, CancellationToken.None)).ToList();

            if (!CoreHelper.RunningOnWin8OrGreater)
            {
                if (TryFetchLibraryDirectories(out var libraries))
                    entries.InsertRange(0, libraries);
                else
                {
                    foreach (var specialFolder in new[]
                    {
                        Environment.SpecialFolder.MyMusic, Environment.SpecialFolder.MyDocuments,
                        Environment.SpecialFolder.MyPictures, Environment.SpecialFolder.MyVideos
                    })
                    {
                        var path = Environment.GetFolderPath(specialFolder);
                        if (TryGetDirectory(path, out var libraryDirectoryEntry))
                            entries.Insert(0, libraryDirectoryEntry);
                    }
                }
            }

            //add missing drives
            foreach (var driveInfo in _drives.Value.Where(x => x.IsReady))
            {
                if (!entries.Any(x => x.Path == driveInfo.RootDirectory.FullName))
                {
                    if (TryGetDirectory(driveInfo.RootDirectory.FullName, out var directoryEntry))
                        entries.Add(directoryEntry);
                }
            }

            return entries;
        }

        public List<DirectoryEntry> GetNamespaceDirectories()
        {
            var result = new Dictionary<DirectoryEntry, int>();
            try
            {
                using (var rootKey = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Classes\CLSID"))
                {
                    if (rootKey != null)
                        foreach (var subKeyName in rootKey.GetSubKeyNames())
                        {
                            using (var possibleEntryRegKey = rootKey.OpenSubKey(subKeyName))
                            {
                                if ((int?) possibleEntryRegKey?.GetValue("System.IsPinnedToNameSpaceTree", 0) != 1)
                                    continue;

                                using (var infoKey = possibleEntryRegKey.OpenSubKey("Instance\\InitPropertyBag"))
                                {
                                    var folder = (string) (infoKey?.GetValue("TargetFolderPath", null) ??
                                                           infoKey?.GetValue("TargetKnownFolder"));
                                    if (folder == null)
                                        continue;

                                    DirectoryEntry entry;
                                    using (var directory = new DirectoryInfoEx(folder))
                                        entry = GetDirectoryEntry(directory, null);

                                    if (entry == null)
                                        continue;

                                    var label = (string) possibleEntryRegKey.GetValue("");
                                    if (!string.IsNullOrEmpty(label))
                                        entry.Label = label;

                                    result.Add(entry,
                                        (int?) possibleEntryRegKey.GetValue("SortOrderIndex", null) ??
                                        int.MaxValue - 1);
                                }
                            }
                        }
                }
            }
            catch (Exception e)
            {
                // Requested registry access is not allowed
                Log.Logger.Warning(e, "Error when accessing registry at SOFTWARE\\Classes\\CLSID");
            }

            result.Add(GetDirectoryEntry(DirectoryInfoEx.RecycleBinDirectory, null), -1);
            return result.OrderBy(x => x.Value).Select(x => x.Key).ToList();
        }

        private FileExplorerEntry GetFileEntry(FileInfoEx fileInfo)
        {
            var result = new FileEntry
            {
                Name = fileInfo.Name,
                Size = fileInfo.Length,
                CreationTime = fileInfo.CreationTimeUtc,
                LastAccess = fileInfo.LastAccessTimeUtc
            };

            return result;
        }

        public DirectoryEntry GetDirectoryEntry(DirectoryInfoEx directory, DirectoryInfoEx parentFolder)
        {
            var directoryEntry = CreateSpecializedDirectory(directory);

            directoryEntry.Name = directory.Name;
            directoryEntry.HasSubFolder = directory.HasSubFolder;
            directoryEntry.CreationTime = directory.CreationTimeUtc;
            directoryEntry.LastAccess = directory.LastAccessTimeUtc;

            if (parentFolder == null && directory.FullName != directory.Name)
                directoryEntry.Path = directory.FullName;
            else if (parentFolder != null && parentFolder.FullName != directory.FullName)
                directoryEntry.Path = directory.FullName;

            return directoryEntry;
        }

        private bool TryFetchLibraryDirectories(out IEnumerable<DirectoryEntry> result)
        {
            try
            {
                var librariesDirectory = new DirectoryInfoEx(KnownFolderIds.Libraries);
                result = librariesDirectory.GetDirectories().Select(x => GetDirectoryEntry(x, null));
                return true;
            }
            catch (Exception e)
            {
                Logger.Warning(e, "Error occurred when trying to fetch library directories");
                result = null;
                return false;
            }
        }

        private bool TryGetDirectory(string path, out DirectoryEntry directoryEntry)
        {
            try
            {
                var directory = new DirectoryInfoEx(path);
                if (!directory.Exists)
                {
                    directoryEntry = null;
                    return false;
                }

                directoryEntry = GetDirectoryEntry(directory, null);
                return true;
            }
            catch (Exception e)
            {
                Logger.Warning(e, "Error occurred when trying to get directory at {path}", path);
                directoryEntry = null;
                return false;
            }
        }

        private DirectoryEntry CreateSpecializedDirectory(DirectoryInfoEx directory)
        {
            if (directory.DirectoryType == DirectoryInfoEx.DirectoryTypeEnum.dtDrive)
            {
                var drive = _drives.Value.FirstOrDefault(x => x.RootDirectory.FullName == directory.FullName);
                if (drive != null)
                {
                    DriveDirectoryEntry driveDirectory;
                    if (drive.IsReady)
                        driveDirectory = new DriveDirectoryEntry
                        {
                            TotalSize = drive.TotalSize,
                            UsedSpace = drive.TotalSize - drive.TotalFreeSpace,
                            DriveType = drive.DriveType
                        };
                    else
                        driveDirectory =
                            new DriveDirectoryEntry { TotalSize = 0, UsedSpace = 0, DriveType = drive.DriveType };

                    SetSpecialFolderAttributes(directory, driveDirectory);
                    return driveDirectory;
                }
            }

            var specialDirectory = new SpecialDirectoryEntry();
            if (SetSpecialFolderAttributes(directory, specialDirectory))
                return specialDirectory;

            return new DirectoryEntry();
        }

        private static bool SetSpecialFolderAttributes(DirectoryInfoEx directory, SpecialDirectoryEntry specialDirectory)
        {
            SetLabel(directory, specialDirectory);
            specialDirectory.IconId = GetFolderIcon(directory);

            return specialDirectory.IconId != 0 || specialDirectory.Label != null || specialDirectory.LabelId != 0 ||
                   specialDirectory.LabelPath != null;
        }

        private static void SetLabel(DirectoryInfoEx directory, SpecialDirectoryEntry directoryEntry)
        {
            if (directory.Name != directory.Label)
                directoryEntry.Label = directory.Label;

            if (directory.TryGetKnownFolderType(out var knownFolder))
            {
                var definition = knownFolder.Definition;
                if (!string.IsNullOrEmpty(definition.LocalizedName))
                {
                    var parts = definition.LocalizedName.TrimStart('@').Split(',');
                    if (parts.Length == 2 && int.TryParse(parts[1], out var id))
                    {
                        directoryEntry.LabelPath = parts[0];
                        directoryEntry.LabelId = id;
                        return;
                    }
                }
            }

            //http://archives.miloush.net/michkap/archive/2007/01/18/1487464.html
            var sb = new StringBuilder(500);
            var len = (uint) sb.Capacity;

            if (NativeMethods.SHGetLocalizedName(directory.FullName, sb, ref len, out var pidsRes) == IntPtr.Zero)
            {
                directoryEntry.LabelPath = sb.ToString();
                directoryEntry.LabelId = pidsRes;
            }
        }

        private static int GetFolderIcon(DirectoryInfoEx directory)
        {
            //must be at first because the files have an icon but that's no available in the administration
            switch (directory.Name)
            {
                case var name when name.Equals("music.library-ms", StringComparison.OrdinalIgnoreCase):
                    return -108;
                case var name when name.Equals("videos.library-ms", StringComparison.OrdinalIgnoreCase):
                    return -189;
                case var name when name.Equals("documents.library-ms", StringComparison.OrdinalIgnoreCase):
                    return -112;
                case var name when name.Equals("pictures.library-ms", StringComparison.OrdinalIgnoreCase):
                    return -113;
            }

            if (directory.TryGetKnownFolderType(out var knownFolder))
            {
                var iconPath = knownFolder.Definition.Icon;
                if (!string.IsNullOrEmpty(iconPath))
                {
                    var parts = iconPath.Trim('"').Split(',');
                    if (parts.Length == 2 && parts[0].EndsWith("imageres.dll", StringComparison.OrdinalIgnoreCase) &&
                        int.TryParse(parts[1], out var resultId))
                        return resultId;
                }

                switch (knownFolder.KnownFolderId)
                {
                    case KnownFolderIds.ComputerFolder:
                        return -109;
                    case KnownFolderIds.RecycleBinFolder:
                        return -54;
                    case KnownFolderIds.NetworkFolder:
                        return -1013;
                }
            }

            var shinfo = new SHFILEINFO();
            NativeMethods.SHGetFileInfo(directory.FullName, 0, ref shinfo, (uint) Marshal.SizeOf(shinfo),
                ShellAPI.SHGFI.ICONLOCATION);

            return shinfo.szDisplayName.EndsWith("imageres.dll", StringComparison.OrdinalIgnoreCase) ? shinfo.iIcon : 0;
        }
    }
}
