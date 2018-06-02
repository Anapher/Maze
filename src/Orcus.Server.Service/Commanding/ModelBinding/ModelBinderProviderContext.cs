using System;

namespace Orcus.Server.Service.Commanding.ModelBinding
{
    public class ModelBinderProviderContext
    {
        /// <summary>
        ///     Gets the <see cref="ModelMetadata" />.
        /// </summary>
        public ModelMetadata Metadata { get; }

        /// <summary>
        ///     Gets the <see cref="IServiceProvider" />.
        /// </summary>
        public virtual IServiceProvider Services { get; }
    }
}