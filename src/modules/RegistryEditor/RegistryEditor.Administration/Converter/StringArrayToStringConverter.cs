using System;
using System.Globalization;
using System.Windows.Data;

namespace RegistryEditor.Administration.Converter
{
    [ValueConversion(typeof(string[]), typeof(string))]
    public class StringArrayToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var array = (string[]) value;
            return string.Join("\r\n", array);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            ((string) value).Split(new[] {"\r", "\n"}, StringSplitOptions.RemoveEmptyEntries);
    }
}