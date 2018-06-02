using System;

namespace Orcus.Modules.Api.Parameters
{
    /// <summary>
    ///     Specifies that an action parameter should be bound using the request services.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter)]
    public class FromServicesAttribute : Attribute, IBindingSourceMetadata
    {
        /// <inheritdoc />
        public BindingSource BindingSource { get; } = BindingSource.Services;
    }
}