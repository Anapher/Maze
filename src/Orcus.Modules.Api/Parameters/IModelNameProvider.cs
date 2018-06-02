namespace Orcus.Modules.Api.Parameters
{
    /// <summary>
    /// Represents an entity which can provide model name as metadata.
    /// </summary>
    public interface IModelNameProvider
    {
        /// <summary>
        /// Model name.
        /// </summary>
        string Name { get; }
    }
}