using Maze.Modules.Api.ModelBinding;

namespace Maze.Service.Commander.Commanding.ModelBinding
{
    /// <summary>
    ///     A context object for <see cref="IModelBinderFactory.CreateBinder" />.
    /// </summary>
    public class ModelBinderFactoryContext
    {
        /// <summary>
        /// Gets or sets the <see cref="Maze.Modules.Api.ModelBinding.BindingInfo"/>.
        /// </summary>
        public BindingInfo BindingInfo { get; set; }

        /// <summary>
        ///     Gets or sets the <see cref="ModelMetadata" />.
        /// </summary>
        public ModelMetadata Metadata { get; set; }

        /// <summary>
        /// Gets or sets the cache token. If <c>non-null</c> the resulting <see cref="IModelBinder"/>
        /// will be cached.
        /// </summary>
        public object CacheToken { get; set; }
    }
}