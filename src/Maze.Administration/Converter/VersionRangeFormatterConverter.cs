using System;
using System.Globalization;
using System.Windows.Data;
using NuGet.Versioning;

namespace Maze.Administration.Converter
{
    public class VersionRangeFormatterConverter : IValueConverter
    {
        private readonly VersionRangeFormatter _formatter = new VersionRangeFormatter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => string.Format(_formatter, "{0:P}", value);

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}