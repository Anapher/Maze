using System;
using System.Drawing;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Data;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using UserInteraction.Administration.Native;
using UserInteraction.Dtos.MessageBox;

namespace UserInteraction.Administration.Converters
{
    [ValueConversion(typeof(string), typeof(BitmapSource))]
    public class SystemIconConverter : IValueConverter
    {
        public object Convert(object value, Type type, object parameter, CultureInfo culture)
        {
            var messageBoxIcon = (MsgBxIcon) value;
            if (messageBoxIcon == MsgBxIcon.None)
                return null;

            SHSTOCKICONID iconId;
            switch (messageBoxIcon)
            {
                case MsgBxIcon.Error:
                    iconId = SHSTOCKICONID.SIID_ERROR;
                    break;
                case MsgBxIcon.Question:
                    iconId = SHSTOCKICONID.SIID_HELP;
                    break;
                case MsgBxIcon.Warning:
                    iconId = SHSTOCKICONID.SIID_WARNING;
                    break;
                case MsgBxIcon.Info:
                    iconId = SHSTOCKICONID.SIID_INFO;
                    break;
                default:
                    return null;
            }

            var sii = new SHSTOCKICONINFO {cbSize = (uint) Marshal.SizeOf(typeof(SHSTOCKICONINFO))};

            Marshal.ThrowExceptionForHR(NativeMethods.SHGetStockIconInfo(iconId, SHGSI.SHGSI_ICON, ref sii));

            var icon = Icon.FromHandle(sii.hIcon);
            var bs = Imaging.CreateBitmapSourceFromHIcon(icon.Handle, Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());
            NativeMethods.DestroyIcon(sii.hIcon);
            return bs;
        }

        public object ConvertBack(object value, Type type, object parameter, CultureInfo culture) =>
            throw new NotSupportedException();
    }
}