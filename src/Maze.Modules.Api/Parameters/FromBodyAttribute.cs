using System;

namespace Maze.Modules.Api.Parameters
{
    /// <summary>
    ///     Specifies that a parameter or property should be bound using the request body.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter)]
    public class FromBodyAttribute : Attribute, IBindingSourceMetadata
    {
        /// <inheritdoc />
        public BindingSource BindingSource { get; } = BindingSource.Body;
    }
}