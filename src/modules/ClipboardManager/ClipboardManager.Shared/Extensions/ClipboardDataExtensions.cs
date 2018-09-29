using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;
using ClipboardManager.Shared.Dtos;
using Newtonsoft.Json;

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

        public static void SetClipboardData(ClipboardData clipboardData)
        {
            var dataObject = new DataObject();

            switch (clipboardData.ValueType)
            {
                case ClipboardValueType.String:
                    dataObject.SetData(clipboardData.Format.ToString(), true, clipboardData.Value);
                    break;
                case ClipboardValueType.StringList:
                    dataObject.SetData(clipboardData.Format.ToString(), true, JsonConvert.DeserializeObject<string[]>(clipboardData.Value));
                    break;
                case ClipboardValueType.Image:
                    if (clipboardData.Value != null)
                        using (var memoryStream = new MemoryStream(Convert.FromBase64String(clipboardData.Value)))
                        {
                            using (var bitmap = Image.FromStream(memoryStream))
                            {
                                dataObject.SetData(clipboardData.Format.ToString(), false, bitmap);
                                Clipboard.SetDataObject(dataObject, true); //important, else we have a problem with disposing the image
                                return;
                            }
                        }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            Clipboard.SetDataObject(dataObject, true);
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