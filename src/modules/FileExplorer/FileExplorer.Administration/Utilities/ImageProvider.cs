using System;
using System.Collections.Generic;
using System.Windows.Media.Imaging;
using FileExplorer.Administration.Cache;
using FileExplorer.Shared.Dtos;
using Microsoft.Extensions.Caching.Memory;
using Maze.Administration.Library.Extensions;

namespace FileExplorer.Administration.Utilities
{
    public class ImageProvider : IImageProvider
    {
        private readonly IMemoryCache _globalCache;
        private readonly IReadOnlyDictionary<int, string> _knownImages;

        public ImageProvider(IMemoryCache globalCache)
        {
            _globalCache = globalCache;
            _knownImages = CreateKnownImages();
        }

        public BitmapImage GetFolderImage(SpecialDirectoryEntry directoryEntry)
        {
            return GetFolderImage(directoryEntry.Name, Math.Abs(directoryEntry.IconId));
        }

        public BitmapImage GetFolderImage(string folderName, int iconId)
        {
            if (!_knownImages.ContainsKey(iconId))
                iconId = FindIconId(folderName);

            var cacheKey = new FolderImageKey {IconId = iconId};
            return _globalCache.GetOrSetValueSafe(cacheKey, TimeSpan.FromMinutes(10), () => LoadImage(iconId));
        }

        private BitmapImage LoadImage(int id)
        {
            var image = new BitmapImage(new Uri(
                $"pack://application:,,,/FileExplorer.Administration;component/Resources/Images/{_knownImages[id]}", UriKind.Absolute));
            image.Freeze();
            return image;
        }

        private static int FindIconId(string folderName)
        {
            switch (folderName)
            {
                case var name when name == null:
                default:
                    return 3;
                case var name when name.Equals("::{20D04FE0-3AEA-1069-A2D8-08002B30309D}", StringComparison.OrdinalIgnoreCase):
                    return 109;
                case var name when name.Equals("Dropbox", StringComparison.OrdinalIgnoreCase):
                    return -2;
                case var name when name.Equals("OneDrive", StringComparison.OrdinalIgnoreCase):
                case var name2 when name2.Equals("{a52bba46-e9e1-435f-b3d9-28daa648c0f6}", StringComparison.OrdinalIgnoreCase):
                    return 1040;
                case var name when name.Equals("Creative Cloud Files", StringComparison.OrdinalIgnoreCase):
                    return -1;
                case var name when name.Equals("Google Drive", StringComparison.OrdinalIgnoreCase):
                    return -3;

            }
        }

        private static IReadOnlyDictionary<int, string> CreateKnownImages()
        {
            return new Dictionary<int, string>
            {
                //Drives
                {56, "Drives/CD-DVD.png"},
                {38, "Drives/CD-DVD.png"},
                {39, "Drives/CD-DVD.png"},
                {40, "Drives/CD-DVD.png"},
                {41, "Drives/CD-DVD.png"},
                {61, "Drives/CD-DVD.png"},
                {36, "Drives/DriveWindows.png"},
                {32, "Drives/Drive.png"},
                {33, "Drives/NetworkDrive.png"},
                {31, "Drives/NetworkDriveDC.png"},
                {37, "Drives/Drive.png"}, //{37, "Drives/OpticalDrive.png"},
                {30, "Drives/Drive.png"}, //{30, "Drives/OpticalDrive.png"},
                {35, "Drives/USB.png"},
                //Folders
                {-1, "Folders/AdobeCloud.png"},
                {178, "Folders/Contacts.png"},
                {181, "Folders/Contacts.png"},
                {183, "Folders/DesktopFolder.png"},
                {112, "Folders/Documents.png"},
                {184, "Folders/Download.png"},
                {-2, "Folders/Dropbox.png"},
                {115, "Folders/Favorite.png"},
                //{5304, "Folders/FilesFolder.png"},
                //{129, "Folders/Fonts.png"},
                //{77, "Folders/Fonts.png"},
                {3, "Folders/Folder.png"},
                {4, "Folders/Folder.png"},
                {186, "Folders/Game.png"},
                {-3, "Folders/GoogleDrive.png"},
                {185, "Folders/Links.png"},
                {108, "Folders/Music.png"},
                {73, "Folders/NetworkFolder.png"},
                {74, "Folders/NetworkFolder.png"},
                {1040, "Folders/OneDrive.png"},
                {1043, "Folders/OneDrive.png"},
                {113, "Folders/Pictures.png"},
                {117, "Folders/RecentPlaces.png"},
                {18, "Folders/Search.png"},
                {1025, "Folders/Search.png"},
                {123, "Folders/User.png"},
                {189, "Folders/Videos.png"},
                //Other
                {110, "Folders/DesktopFolder.png"},
                //{1013, "Other/HomeGroup.png"},
                {109, "Other/ThisPC.png"},
                {54, "Other/TrashFull.png"},
                {55, "Other/TrashFull.png"}
            };
        }
    }
}