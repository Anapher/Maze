using System;
using System.Windows;
using System.Windows.Controls;

namespace Tasks.Infrastructure.Administration.Controls.PropertyGrid.Editors
{
    public class TextBoxEditor : PropertyEditor<TextBox>
    {
        public override int Priority { get; } = 0;

        public override bool IsSupported(PropertyItem propertyItem, Type propertyType)
        {
            if (propertyType == typeof(string))
                return true;

            return false;
        }

        protected override DependencyProperty GetDependencyProperty() => TextBox.TextProperty;

        protected override void InitializeControl(PropertyEditorContext<TextBox> context)
        {
            base.InitializeControl(context);
            context.Editor.BorderThickness = new Thickness(0);
            context.Editor.MinHeight = 20;
        }
    }
}