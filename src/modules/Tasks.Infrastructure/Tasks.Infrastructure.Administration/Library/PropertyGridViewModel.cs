using System.Collections.Generic;
using Prism.Mvvm;
using Tasks.Infrastructure.Administration.PropertyGrid;

namespace Tasks.Infrastructure.Administration.Library
{
    public abstract class PropertyGridViewModel : BindableBase, IProvideEditableProperties
    {
        protected PropertyGridViewModel()
        {
            Properties = new List<IProperty>();
        }

        public List<IProperty> Properties { get; }
    }
}