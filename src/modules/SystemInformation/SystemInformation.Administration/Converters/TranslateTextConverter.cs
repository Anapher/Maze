using System;
using System.Globalization;
using System.Windows.Data;
using Unclassified.TxLib;

namespace SystemInformation.Administration.Converters
{
    [ValueConversion(typeof(string), typeof(string))]
    public class TranslateTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => Tx.T("SystemInformation:Labels." + value);

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}