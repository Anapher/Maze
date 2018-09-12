#pragma warning disable CS0728 // Possibly incorrect assignment to local which is the argument to a using or lock statement. Im not fucking dumb.

using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace TaskManager.Client.Utilities
{
    public class FileUtilities
    {
        public static byte[] GetFileIcon(string filename, int size)
        {
            Bitmap bitmap;

            using (var icon = IconTools.GetIconForFile(filename, size <= 16 ? ShellIconSize.SmallIcon : ShellIconSize.LargeIcon))
            {
                if (icon == null)
                    return null;

                bitmap = icon.ToBitmap();
            }

            if (bitmap.Size.Width != size)
            {
                using (bitmap)
                    bitmap = bitmap.ResizeImage(size, size);
            }

            using (bitmap)
            {
                using (var ms = new MemoryStream())
                {
                    bitmap.Save(ms, ImageFormat.Png);
                    return ms.ToArray();
                }
            }
        }
    }
}