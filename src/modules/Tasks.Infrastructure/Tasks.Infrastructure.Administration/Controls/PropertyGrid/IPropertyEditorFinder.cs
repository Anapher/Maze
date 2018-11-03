using System.Windows;

namespace Tasks.Infrastructure.Administration.Controls.PropertyGrid
{
    public interface IPropertyEditorFinder
    {
        FrameworkElement FindAndCreateEditor(PropertyItem propertyItem);
    }
}