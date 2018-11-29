using System.Collections.Generic;
using Prism.Mvvm;
using Tasks.Infrastructure.Administration.PropertyGrid;

namespace Tasks.Infrastructure.Administration.Library
{
    /// <summary>
    ///     The base class for a property grid based view model/view. The view will automatically be resolved
    /// </summary>
    public abstract class PropertyGridViewModel : BindableBase, IProvideEditableProperties
    {
        /// <summary>
        ///     Initialize a new instance of <see cref="PropertyGridViewModel"/>
        /// </summary>
        protected PropertyGridViewModel()
        {
            Properties = new List<IProperty>();
        }

        /// <summary>
        ///     The properties for the service
        /// </summary>
        public List<IProperty> Properties { get; }
    }
}