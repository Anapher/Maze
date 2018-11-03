using System;
using System.Windows;
using System.Windows.Media;
using Xceed.Wpf.Toolkit;

namespace Tasks.Infrastructure.Administration.Controls.PropertyGrid.Editors
{
    public class ColorEditor : PropertyEditor<ColorPicker>
    {
        public override int Priority { get; } = 0;

        protected override DependencyProperty GetDependencyProperty() => ColorPicker.SelectedColorProperty;

        public override bool IsSupported(PropertyItem propertyItem, Type propertyType)
        {
            if (propertyType == typeof(Color) || propertyType == typeof(Color?))
                return true;

            return false;
        }

        protected override ColorPicker CreateEditor(PropertyEditorContext<ColorPicker> context) => new PropertyGridEditorColorPicker();
    }

    public class PropertyGridEditorColorPicker : ColorPicker
    {
        static PropertyGridEditorColorPicker()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PropertyGridEditorColorPicker),
                new FrameworkPropertyMetadata(typeof(PropertyGridEditorColorPicker)));
        }
    }
}