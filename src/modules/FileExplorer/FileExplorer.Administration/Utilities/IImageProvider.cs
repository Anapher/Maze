using System.Windows.Media.Imaging;
using FileExplorer.Shared.Dtos;

namespace FileExplorer.Administration.Utilities
{
    public interface IImageProvider
    {
        BitmapImage GetFolderImage(SpecialDirectoryEntry directoryEntry);
        BitmapImage GetFolderImage(string folderName, int iconId);
    }
}