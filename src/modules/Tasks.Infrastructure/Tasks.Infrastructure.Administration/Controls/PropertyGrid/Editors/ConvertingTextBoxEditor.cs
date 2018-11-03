using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;
using Tasks.Infrastructure.Administration.PropertyGrid;

namespace Tasks.Infrastructure.Administration.Controls.PropertyGrid.Editors
{
    public class ConvertingTextBoxEditor : TextBoxEditor
    {
        public override int Priority { get; } = -100;

        protected override IValueConverter CreateValueConverter(PropertyEditorContext<TextBox> context)
        {
            var typeConverter = TypeDescriptor.GetConverter(context.Property.PropertyType);
            return new StringToTypeConverter(typeConverter, context.Property);
        }

        public override bool IsSupported(PropertyItem propertyItem, Type propertyType)
        {
            var typeConverter = TypeDescriptor.GetConverter(propertyType);
            return typeConverter.CanConvertFrom(typeof(string));
        }

        private class StringToTypeConverter : IValueConverter
        {
            private readonly TypeConverter _typeConverter;
            private readonly IProperty _property;

            public StringToTypeConverter(TypeConverter typeConverter, IProperty property)
            {
                _typeConverter = typeConverter;
                _property = property;
            }

            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                return _typeConverter.ConvertToString(value);
            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                try
                {
                    return _typeConverter.ConvertFromString((string) value);
                }
                catch (Exception)
                {
                    return _property.Value;
                }
            }
        }
    }
}