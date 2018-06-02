using System;

namespace Orcus.Server.Service.Commanding.ModelBinding
{
    public abstract class ModelBinderProviderContext
    {
        /// <summary>
        ///     Gets the <see cref="ModelMetadata" />.
        /// </summary>
        public abstract ModelMetadata Metadata { get; }

        /// <summary>
        ///     Gets the <see cref="IServiceProvider" />.
        /// </summary>
        public abstract IServiceProvider Services { get; }
    }
}