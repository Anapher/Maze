using System;
using System.Globalization;
using System.Reflection;
using System.Windows;
using System.Windows.Data;
using MahApps.Metro.Controls;
using Tasks.Infrastructure.Administration.PropertyGrid.Attributes;

namespace Tasks.Infrastructure.Administration.Controls.PropertyGrid.Editors
{
    public abstract class UpDownEditor<T> : PropertyEditor<NumericUpDown> where T : IConvertible
    {
        private readonly double _minValue;
        private readonly double _maxValue;
        private readonly bool _isDecimal;

        protected UpDownEditor(double minValue, double maxValue)
        {
            _minValue = minValue;
            _maxValue = maxValue;
        }

        protected UpDownEditor(double minValue, double maxValue, bool isDecimal)
        {
            _minValue = minValue;
            _maxValue = maxValue;
            _isDecimal = isDecimal;
        }

        public override int Priority { get; } = 0;

        public override bool IsSupported(PropertyItem propertyItem, Type propertyType) => IsValueTypeOrNullableEquivalent(propertyType, typeof(T));

        protected override DependencyProperty GetDependencyProperty()
        {
            return NumericUpDown.ValueProperty;
        }

        protected override NumericUpDown CreateEditor(PropertyEditorContext<NumericUpDown> context) => new PropertyGridEditorNumericUpDown();

        protected override void InitializeControl(PropertyEditorContext<NumericUpDown> context)
        {
            base.InitializeControl(context);

            var numericAttribute = context.Property.PropertyType.GetCustomAttribute<NumericValueAttribute>();

            context.Editor.Maximum = numericAttribute?.Maximum < _maxValue ? numericAttribute.Maximum : _maxValue;
            context.Editor.Minimum = numericAttribute?.Minimum > _minValue ? numericAttribute.Minimum : _minValue;
            context.Editor.BorderThickness = new Thickness(0);
            context.Editor.TextAlignment = TextAlignment.Left;
            context.Editor.MinHeight = 20;
            context.Editor.UpDownButtonsWidth = 20;

            if (numericAttribute?.StringFormat != null)
                context.Editor.StringFormat = numericAttribute.StringFormat;

            if (_isDecimal)
            {
                context.Editor.NumericInputMode = NumericInput.Decimal;
                context.Editor.Interval = .1;
            }
            else
            {
                context.Editor.NumericInputMode = NumericInput.Numbers;
            }
        }

        protected override IValueConverter CreateValueConverter(PropertyEditorContext<NumericUpDown> context)
        {
            return new BasicTypeConverter<T>();
        }

        private class BasicTypeConverter<TResult> : IValueConverter where  TResult : IConvertible
        {
            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                return value;
            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                return value == null ? default : (TResult) System.Convert.ChangeType(value, typeof (TResult));
            }
        }
    }

    public class PropertyGridEditorNumericUpDown : NumericUpDown
    {
        static PropertyGridEditorNumericUpDown()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PropertyGridEditorNumericUpDown),
                new FrameworkPropertyMetadata(typeof(PropertyGridEditorNumericUpDown)));
        }
    }
}