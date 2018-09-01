using System;
using System.Globalization;
using System.Windows.Data;

namespace FileExplorer.Administration.Converters
{
    [ValueConversion(typeof(TimeSpan), typeof(string))]
    public class TimeSpanToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var timespan = (TimeSpan) value;
            if (timespan.Hours > 0)
                return ((int) timespan.TotalHours).ToString("00") + ":" + timespan.ToString("mm\\:ss");
            return timespan.ToString("mm\\:ss");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            throw new NotImplementedException();
    }
}