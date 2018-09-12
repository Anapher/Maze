using System.IO;
using System.Windows.Media.Imaging;

namespace TaskManager.Administration.Utilities
{
    public static class ImageUtilities
    {
        public static BitmapImage GetBitmapImage(byte[] data)
        {
            using (var memoryStream = new MemoryStream(data, false))
            {
                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.StreamSource = memoryStream;
                bitmapImage.EndInit();

                return bitmapImage;
            }
        }
    }
}