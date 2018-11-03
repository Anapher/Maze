using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Tasks.Infrastructure.Administration.Controls.PropertyGrid
{
    public abstract class PropertyEditor<TEditor> : IPropertyEditorFactory where TEditor : FrameworkElement, new()
    {
        public abstract int Priority { get; }

        public FrameworkElement CreateEditor(PropertyItem propertyItem)
        {
            if (!IsSupported(propertyItem, propertyItem.Property.PropertyType))
                return null;

            var context = new PropertyEditorContext<TEditor>(propertyItem.Property, propertyItem);

            context.Editor = CreateEditor(context);
            context.ValueProperty = GetDependencyProperty();

            InitializeControl(context);
            ResolveValueBinding(context);

            return context.Editor;
        }

        public abstract bool IsSupported(PropertyItem propertyItem, Type propertyType);

        protected virtual TEditor CreateEditor(PropertyEditorContext<TEditor> context)
        {
            return new TEditor();
        }

        protected virtual void ResolveValueBinding(PropertyEditorContext<TEditor> context)
        {
            var binding = new Binding("Value")
            {
                Source = context.PropertyItem,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                Mode = context.PropertyItem.IsReadOnly ? BindingMode.OneWay : BindingMode.TwoWay,
                Converter = CreateValueConverter(context)
            };

            BindingOperations.SetBinding(context.Editor, context.ValueProperty, binding);
        }

        protected virtual IValueConverter CreateValueConverter(PropertyEditorContext<TEditor> context)
        {
            return new NullAsDefaultConverter(context.Property.PropertyType);
        }

        protected virtual void InitializeControl(PropertyEditorContext<TEditor> context)
        {
        }

        protected abstract DependencyProperty GetDependencyProperty();

        protected static bool IsValueTypeOrNullableEquivalent(Type checkingType, Type valueType)
        {
            return checkingType == valueType || Nullable.GetUnderlyingType(checkingType) == valueType;
        }

        private class NullAsDefaultConverter : IValueConverter
        {
            private readonly Type _type;

            public NullAsDefaultConverter(Type type)
            {
                _type = type;
            }

            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                return value;
            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                if (value == null && _type.IsValueType)
                    return Activator.CreateInstance(_type);

                return value;
            }
        }
    }
}