namespace Orcus.Modules.Api.Parameters
{
    /// <summary>
    /// Metadata which specificies the data source for model binding.
    /// </summary>
    public interface IBindingSourceMetadata
    {
        /// <summary>
        /// Gets the <see cref="BindingSource"/>. 
        /// </summary>
        /// <remarks>
        /// The <see cref="BindingSource"/> is metadata which can be used to determine which data
        /// sources are valid for model binding of a property or parameter.
        /// </remarks>
        BindingSource BindingSource { get; }
    }
}