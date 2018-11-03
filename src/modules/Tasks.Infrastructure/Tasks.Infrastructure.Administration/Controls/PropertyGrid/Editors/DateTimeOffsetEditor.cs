using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using Xceed.Wpf.Toolkit;

namespace Tasks.Infrastructure.Administration.Controls.PropertyGrid.Editors
{
    public class DateTimeOffsetEditor : PropertyEditor<DateTimeUpDown>
    {
        public override int Priority { get; } = 0;

        public override bool IsSupported(PropertyItem propertyItem, Type propertyType)
        {
            if (propertyType == typeof(DateTimeOffset) || propertyType == typeof(DateTimeOffset?))
                return true;

            return false;
        }

        protected override DependencyProperty GetDependencyProperty() => DateTimeUpDown.ValueProperty;

        protected override DateTimeUpDown CreateEditor(PropertyEditorContext<DateTimeUpDown> context) => new PropertyGridEditorDateTimeUpDown();

        protected override IValueConverter CreateValueConverter(PropertyEditorContext<DateTimeUpDown> context) =>
            new DateTimeOffsetConverter(!context.Property.PropertyType.IsValueType);

        protected override void InitializeControl(PropertyEditorContext<DateTimeUpDown> context)
        {
            base.InitializeControl(context);

            if (context.Property.PropertyType.IsValueType && (DateTimeOffset) context.Property.Value == default)
                context.Property.Value = DateTimeOffset.Now;
        }

        private class DateTimeOffsetConverter : IValueConverter
        {
            private readonly bool _nullable;

            public DateTimeOffsetConverter(bool nullable)
            {
                _nullable = nullable;
            }

            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                if (value is DateTimeOffset dateTimeOffset)
                    return dateTimeOffset.DateTime;

                return null;
            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                if (value is DateTime dateTime)
                    return new DateTimeOffset(dateTime);

                return _nullable ? (DateTimeOffset?) null : default(DateTimeOffset);
            }
        }
    }
}