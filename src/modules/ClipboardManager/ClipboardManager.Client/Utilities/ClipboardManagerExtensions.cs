using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using ClipboardManager.Shared.Dtos;
using Newtonsoft.Json;

namespace ClipboardManager.Client.Utilities
{
    public static class ClipboardManagerExtensions
    {
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
    }
}