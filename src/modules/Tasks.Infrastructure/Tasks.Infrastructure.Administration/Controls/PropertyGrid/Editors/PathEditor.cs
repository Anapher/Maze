using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using Tasks.Infrastructure.Administration.Controls.PropertyGrid.Controls;
using Tasks.Infrastructure.Administration.PropertyGrid.Attributes;

namespace Tasks.Infrastructure.Administration.Controls.PropertyGrid.Editors
{
    public class PathEditor : PropertyEditor<PathBox>
    {
        public override int Priority { get; } = 10;

        public override bool IsSupported(PropertyItem propertyItem, Type propertyType)
        {
            if (propertyType == typeof(string) && propertyType.GetCustomAttribute<PathAttribute>() != null)
                return true;

            return false;
        }

        protected override DependencyProperty GetDependencyProperty() => TextBox.TextProperty;

        protected override void InitializeControl(PropertyEditorContext<PathBox> context)
        {
            base.InitializeControl(context);

            var attribute = context.Property.PropertyType.GetCustomAttribute<PathAttribute>();

            context.Editor.IsSelectingFile = attribute.PathMode == PathMode.File;
            context.Editor.Filter = attribute.Filter;
            context.Editor.BorderThickness = new Thickness(0);
            context.Editor.MinHeight = 20;
        }
    }
}