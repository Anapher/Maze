using System;
using System.Windows;
using System.Windows.Controls;

namespace Tasks.Infrastructure.Administration.Controls.PropertyGrid.Editors
{
    public class TextBlockEditor : PropertyEditor<TextBlock>
    {
        public override int Priority { get; } = -100000;
        public override bool IsSupported(PropertyItem propertyItem, Type propertyType) => true;

        protected override DependencyProperty GetDependencyProperty()
        {
            return TextBlock.TextProperty;
        }
    }
}