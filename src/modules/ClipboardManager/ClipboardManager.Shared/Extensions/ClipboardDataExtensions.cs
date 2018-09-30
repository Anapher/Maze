using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

using ClipboardManager.Shared.Dtos;
using Newtonsoft.Json;

#if WPF
using System.Windows;
#else
using System.Windows.Forms;
#endif

namespace ClipboardManager.Shared.Extensions
{
    public static class ClipboardDataExtensions
    {
        private const int BitmapHeight = 200;

        public static ClipboardData FromDataObject(IDataObject dataObject)
        {
            var format = GetClipboardFormat(dataObject);
            string resultValue = null;
            var valueType = ClipboardValueType.String;

            switch (dataObject)
            {
                case var data when data.GetDataPresent(DataFormats.Bitmap, true):
                    using (var bitmap = data.GetData(DataFormats.Bitmap) as Image)
                    {
                        if (bitmap != null)
                            using (var memoryStream = new MemoryStream())
                            {
                                if (bitmap.Height > BitmapHeight)
                                    using (
                                        var resizedBitmap =
                                            bitmap.ResizeImage((int)((float)bitmap.Width / bitmap.Height * BitmapHeight),
                                                BitmapHeight))
                                        resizedBitmap.Save(memoryStream, ImageFormat.Jpeg);
                                else
                                    bitmap.Save(memoryStream, ImageFormat.Jpeg);

                                resultValue = Convert.ToBase64String(memoryStream.ToArray());
                            }

                        format = format ?? ClipboardDataFormat.Bitmap;
                        valueType = ClipboardValueType.Image;
                    }
                    break;
                case var data when data.GetDataPresent(DataFormats.Text, true): //that includes also UnicodeText, Rtf, Html, CommaSeparatedValue
                    resultValue = dataObject.GetData(DataFormats.Text, true) as string;
                    format = format ?? ClipboardDataFormat.Text;
                    valueType = ClipboardValueType.String;
                    break;
                case var data when data.GetDataPresent(DataFormats.FileDrop, true):
                    resultValue = JsonConvert.SerializeObject(dataObject.GetData(DataFormats.FileDrop) as string[]);
                    format = format ?? ClipboardDataFormat.FileDrop;
                    valueType = ClipboardValueType.StringList;
                    break;
                default:
                    format = format ?? (ClipboardDataFormat) int.MaxValue;
                    break;
            }

            return new ClipboardData {Format = format.Value, Value = resultValue, ValueType = valueType};
        }

        private static ClipboardDataFormat? GetClipboardFormat(IDataObject dataObject)
        {
            foreach (var f in Enum.GetNames(typeof(ClipboardDataFormat)))
                if (dataObject.GetDataPresent(f))
                    return (ClipboardDataFormat) Enum.Parse(typeof(ClipboardDataFormat), f);

            return null;
        }
    }
}