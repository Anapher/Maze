using System.Collections.Generic;

namespace Tasks.Infrastructure.Administration.PropertyGrid
{
    /// <summary>
    ///     The properties of the object can be edited
    /// </summary>
    public interface IProvideEditableProperties
    {
        /// <summary>
        ///     The properties to edit
        /// </summary>
        List<IProperty> Properties { get; }
    }
}