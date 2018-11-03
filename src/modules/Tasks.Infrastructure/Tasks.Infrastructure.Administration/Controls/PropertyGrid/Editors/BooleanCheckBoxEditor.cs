using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Tasks.Infrastructure.Administration.PropertyGrid.Attributes;

namespace Tasks.Infrastructure.Administration.Controls.PropertyGrid.Editors
{
    public class BooleanCheckBoxEditor : PropertyEditor<CheckBox>
    {
        public override int Priority { get; } = 10;

        public override bool IsSupported(PropertyItem propertyItem, Type propertyType)
        {
            if (IsValueTypeOrNullableEquivalent(propertyType, typeof(bool)) &&
                propertyItem.Property.PropertyInfo.GetCustomAttribute<CheckBoxBooleanAttribute>() != null)
                return true;

            return false;
        }

        protected override DependencyProperty GetDependencyProperty() => ToggleButton.IsCheckedProperty;

        protected override void InitializeControl(PropertyEditorContext<CheckBox> context)
        {
            base.InitializeControl(context);

            if (Nullable.GetUnderlyingType(context.Property.PropertyType) != null)
                context.Editor.IsThreeState = true;

            context.Editor.Margin = new Thickness(10, 0, 0, 0);
        }
    }
}