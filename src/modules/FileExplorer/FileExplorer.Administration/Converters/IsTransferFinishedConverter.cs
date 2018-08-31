using System;
using System.Globalization;
using System.Windows.Data;
using FileExplorer.Administration.ViewModels.Explorer;

namespace FileExplorer.Administration.Converters
{
    [ValueConversion(typeof(FileTransferState), typeof(bool))]
    public class IsTransferFinishedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var state = (FileTransferState) value;
            return state > FileTransferState.Transferring;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}