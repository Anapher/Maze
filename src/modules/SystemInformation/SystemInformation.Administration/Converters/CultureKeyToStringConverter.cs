using System;
using System.Globalization;
using System.Windows.Data;

namespace SystemInformation.Administration.Converters
{
    [ValueConversion(typeof(string), typeof(string))]
    public class CultureKeyToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || !(value is string s))
                return null;

            var cultureInfo = CultureInfo.GetCultureInfo(s);
            return $"{cultureInfo.DisplayName} ({cultureInfo.TwoLetterISOLanguageName})";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}