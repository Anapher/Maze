using System;

namespace Orcus.Modules.Api.Parameters
{
    /// <summary>
    ///     Specifies that a parameter or property should be bound using the request headers.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter)]
    public class FromHeaderAttribute : Attribute, IBindingSourceMetadata, IModelNameProvider
    {
        /// <inheritdoc />
        public BindingSource BindingSource => BindingSource.Header;

        /// <inheritdoc />
        public string Name { get; set; }
    }
}