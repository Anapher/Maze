using System;
using System.Windows;
using Tasks.Infrastructure.Administration.Controls.PropertyGrid;
using Tasks.Infrastructure.Administration.PropertyGrid;

namespace Tasks.Infrastructure.Administration.Core
{
    public class PropertyGridViewProvider : IViewProviderForAll
    {
        private readonly IPropertyEditorFinder _propertyEditorFinder;

        public PropertyGridViewProvider(IPropertyEditorFinder propertyEditorFinder)
        {
            _propertyEditorFinder = propertyEditorFinder;
        }

        public int Priority { get; } = -10;

        public UIElement GetView(object viewModel, IServiceProvider context)
        {
            if (viewModel is IProvideEditableProperties provideEditableProperties)
                return new Controls.PropertyGrid.PropertyGrid
                {
                    PropertiesProvider = provideEditableProperties, PropertyEditorFinder = _propertyEditorFinder
                };

            return null;
        }
    }
}