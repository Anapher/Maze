using System;
using System.Windows;
using Xceed.Wpf.Toolkit;

namespace Tasks.Infrastructure.Administration.Controls.PropertyGrid.Editors
{
    public class DateTimeEditor : PropertyEditor<DateTimeUpDown>
    {
        public override int Priority { get; } = 0;

        public override bool IsSupported(PropertyItem propertyItem, Type propertyType)
        {
            if (propertyType == typeof(DateTime) || propertyType == typeof(DateTime?))
                return true;

            return false;
        }

        protected override DependencyProperty GetDependencyProperty()
        {
            return DateTimeUpDown.ValueProperty;
        }

        protected override DateTimeUpDown CreateEditor(PropertyEditorContext<DateTimeUpDown> context) => new PropertyGridEditorDateTimeUpDown();

        protected override void InitializeControl(PropertyEditorContext<DateTimeUpDown> context)
        {
            base.InitializeControl(context);

            if (context.Property.PropertyType.IsValueType && (DateTime) context.Property.Value == default)
                context.Property.Value = DateTime.Now;
        }
    }

    public class PropertyGridEditorDateTimeUpDown : DateTimeUpDown
    {
        static PropertyGridEditorDateTimeUpDown()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PropertyGridEditorDateTimeUpDown),
                new FrameworkPropertyMetadata(typeof(PropertyGridEditorDateTimeUpDown)));
        }
    }
}