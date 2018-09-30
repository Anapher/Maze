using System;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using ClipboardManager.Shared.Dtos;
using Newtonsoft.Json;

namespace ClipboardManager.Administration.Utilities
{
    public static class ClipboardExtensions
    {
        public static void SetClipboardData(ClipboardData clipboardData)
        {
            var dataObject = new DataObject();

            switch (clipboardData.ValueType)
            {
                case ClipboardValueType.String:
                    dataObject.SetData(clipboardData.Format.ToString(), clipboardData.Value, true);
                    break;
                case ClipboardValueType.StringList:
                    dataObject.SetData(clipboardData.Format.ToString(), JsonConvert.DeserializeObject<string[]>(clipboardData.Value), true);
                    break;
                case ClipboardValueType.Image:
                    if (clipboardData.Value != null)
                        using (var memoryStream = new MemoryStream(Convert.FromBase64String(clipboardData.Value)))
                        {
                            var image = new BitmapImage();
                            image.BeginInit();
                            image.CacheOption = BitmapCacheOption.OnLoad;
                            image.StreamSource = memoryStream;
                            image.EndInit();
                            dataObject.SetImage(image);
                        }

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            Clipboard.SetDataObject(dataObject, true);
        }
    }
}