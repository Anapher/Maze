using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Tasks.Infrastructure.Administration.Controls.PropertyGrid;

namespace Tasks.Infrastructure.Administration.Core
{
    public class DefaultPropertyEditorFinder : IPropertyEditorFinder
    {
        private readonly IReadOnlyList<IPropertyEditorFactory> _factories;

        public DefaultPropertyEditorFinder(IEnumerable<IPropertyEditorFactory> factories)
        {
            _factories = factories.OrderByDescending(x => x.Priority).ToList();
        }

        public FrameworkElement FindAndCreateEditor(PropertyItem propertyItem)
        {
            return _factories.Select(x => x.CreateEditor(propertyItem)).FirstOrDefault(x => x != null);
        }
    }
}