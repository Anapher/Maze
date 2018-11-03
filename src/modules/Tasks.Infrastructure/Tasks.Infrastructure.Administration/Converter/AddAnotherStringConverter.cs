using System;
using System.Globalization;
using System.Windows.Data;
using Unclassified.TxLib;

namespace Tasks.Infrastructure.Administration.Converter
{
    public class AddAnotherStringConverter : IValueConverter
    {
        public static AddAnotherStringConverter Instance { get; } = new AddAnotherStringConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
            Tx.T("TasksInfrastructure:CreateTask.AddAnother", "entry", value.ToString());

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}