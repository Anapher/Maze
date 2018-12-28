using System;
using System.Globalization;
using System.Windows.Data;

namespace Maze.Administration.Converter
{
    [ValueConversion(typeof(string), typeof(string))]
    public class NoNewLinesConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var s = (string) value;
            return s.Replace(Environment.NewLine, "  ");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            throw new NotImplementedException();
    }
}