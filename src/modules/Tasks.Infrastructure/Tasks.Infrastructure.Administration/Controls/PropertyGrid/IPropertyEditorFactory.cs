using System.Windows;

namespace Tasks.Infrastructure.Administration.Controls.PropertyGrid
{
    public interface IPropertyEditorFactory
    {
        int Priority { get; }
        FrameworkElement CreateEditor(PropertyItem propertyItem);
    }
}