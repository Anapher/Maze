using System.Windows;
using Tasks.Infrastructure.Administration.PropertyGrid;

namespace Tasks.Infrastructure.Administration.Controls.PropertyGrid
{
    public class PropertyEditorContext<TEditor> where TEditor : FrameworkElement, new()
    {
        public PropertyEditorContext(IProperty property, PropertyItem propertyItem)
        {
            Property = property;
            PropertyItem = propertyItem;
        }

        public IProperty Property { get; }
        public PropertyItem PropertyItem { get; }

        public DependencyProperty ValueProperty { get; set; }
        public TEditor Editor { get; set; }
    }
}