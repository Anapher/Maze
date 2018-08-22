using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FileExplorer.Client.Native;
using FileExplorer.Shared.Dtos;
using Orcus.Modules.Api.Utilities;
using ShellDll;

namespace FileExplorer.Client.Utilities
{
    public class DirectoryHelper
    {
        private readonly Lazy<DriveInfo[]> _drives;

        public DirectoryHelper()
        {
            _drives = new Lazy<DriveInfo[]>(DriveInfo.GetDrives, LazyThreadSafetyMode.ExecutionAndPublication);
        }

        public Task<IEnumerable<FileExplorerEntry>> GetEntries(string directoryPath)
        {
            using (var directory = new DirectoryInfoEx(directoryPath))
                return GetEntries(directory);
        }

        public Task<IEnumerable<FileExplorerEntry>> GetEntries(DirectoryInfoEx directory)
        {
            var entries = directory.EnumerateFileSystemInfos("*", SearchOption.TopDirectoryOnly);
            return TaskCombinators.ThrottledAsync(entries, (entry, token) => Task.Run(() =>
            {
                using (entry)
                {
                    if (entry.IsFolder)
                        return GetDirectoryEntry((DirectoryInfoEx) entry);

                    return GetFileEntry((FileInfoEx) entry);
                }
            }, token), CancellationToken.None);
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

        private FileExplorerEntry GetDirectoryEntry(DirectoryInfoEx directory)
        {
            SpecialDirectoryEntry directoryEntry;

            if (directory.DirectoryType == DirectoryInfoEx.DirectoryTypeEnum.dtDrive)
            {
                var drive = _drives.Value.FirstOrDefault(x => x.RootDirectory.FullName == directory.FullName);
                if (drive != null)
                {
                    if (drive.IsReady)
                        directoryEntry = new DriveDirectoryEntry
                        {
                            TotalSize = drive.TotalSize,
                            UsedSpace = drive.TotalSize - drive.TotalFreeSpace,
                            DriveType = drive.DriveType
                        };
                    else
                        directoryEntry =
                            new DriveDirectoryEntry {TotalSize = 0, UsedSpace = 0, DriveType = drive.DriveType};
                } else directoryEntry = new SpecialDirectoryEntry();
            }
            else directoryEntry = new SpecialDirectoryEntry();

            directoryEntry.Name = directory.Name;
            directoryEntry.HasSubFolder = directory.HasSubFolder;
            directoryEntry.CreationTime = directory.CreationTimeUtc;
            directoryEntry.LastAccess = directory.LastAccessTimeUtc;

            SetSpecialFolderAttributes(directory, directoryEntry);

            return directoryEntry;
        }

        private static void SetSpecialFolderAttributes(DirectoryInfoEx directory, SpecialDirectoryEntry directoryEntry)
        {
            if (directory.Name != directory.Label)
                directoryEntry.Label = directory.Label;

            if (!string.IsNullOrEmpty(directory.KnownFolderType?.Definition.LocalizedName))
            {
                var parts = directory.KnownFolderType.Definition.LocalizedName.TrimStart('@').Split(',');
                if (parts.Length == 2 && int.TryParse(parts[1], out var id))
                {
                    directoryEntry.LabelPath = parts[0];
                    directoryEntry.LabelId = id;
                    return;
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

            directoryEntry.IconId = GetFolderIcon(directory);
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

            var iconPath = directory.KnownFolderType?.Definition.Icon;
            if (!string.IsNullOrEmpty(iconPath))
            {
                var parts = iconPath.Trim('"').Split(',');
                if (parts.Length == 2 && parts[0].EndsWith("imageres.dll", StringComparison.OrdinalIgnoreCase) &&
                    int.TryParse(parts[1], out var resultId))
                    return resultId;
            }

            switch (directory.KnownFolderType?.KnownFolderId)
            {
                case KnownFolderIds.ComputerFolder:
                    return -109;
                case KnownFolderIds.RecycleBinFolder:
                    return -54;
                case KnownFolderIds.NetworkFolder:
                    return -1013;
            }

            var shinfo = new ShellAPI.SHFILEINFO();
            NativeMethods.SHGetFileInfo(directory.FullName, 0, ref shinfo, (uint) Marshal.SizeOf(shinfo),
                ShellAPI.SHGFI.ICONLOCATION);

            return shinfo.szDisplayName.EndsWith("imageres.dll", StringComparison.OrdinalIgnoreCase) ? shinfo.iIcon : 0;
        }
    }
}
