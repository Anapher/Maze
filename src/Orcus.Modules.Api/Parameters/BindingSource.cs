namespace Orcus.Modules.Api.Parameters
{
    /// <summary>
    ///     A metadata object representing a source of data for model binding.
    /// </summary>
    public enum BindingSource
    {
        /// <summary>
        ///     A <see cref="BindingSource" /> for the request query-string.
        /// </summary>
        Query,

        /// <summary>
        ///     A <see cref="BindingSource" /> for the request headers.
        /// </summary>
        Header,

        /// <summary>
        ///     A <see cref="BindingSource" /> for the request body.
        /// </summary>
        Body,

        /// <summary>
        ///     A <see cref="BindingSource" /> for model binding. Includes form-data, query-string
        ///     and route data from the request.
        /// </summary>
        ModelBinding,

        /// <summary>
        ///     A <see cref="BindingSource" /> for request services.
        /// </summary>
        Services,

        /// <summary>
        ///     A <see cref="BindingSource" /> for path parameters
        /// </summary>
        Path
    }
}