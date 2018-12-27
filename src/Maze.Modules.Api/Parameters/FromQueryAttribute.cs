using System;

namespace Orcus.Modules.Api.Parameters
{
    /// <summary>
    /// Specifies that a parameter or property should be bound using the request query string.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter)]
    public class FromQueryAttribute : Attribute, IBindingSourceMetadata, IModelNameProvider
    {
        /// <inheritdoc />
        public string Name { get; set; }

        /// <inheritdoc />
        public BindingSource BindingSource { get; } = BindingSource.Query;
    }
}