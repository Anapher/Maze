namespace Orcus.Administration.Library.Models
{
    /// <summary>
    ///     A factory that will create multiple, equal instances
    /// </summary>
    public interface IIconFactory
    {
        /// <summary>
        ///     Create a new instance of the same data
        /// </summary>
        /// <returns>Return the new instance.</returns>
        object Create();
    }
}