using System;
using System.Threading;

namespace Orcus.Service.Commander.Commanding.ModelBinding.Binders
{
    /// <summary>
    ///     An <see cref="IModelBinderProvider" /> for <see cref="CancellationToken" />.
    /// </summary>
    public class CancellationTokenModelBinderProvider : IModelBinderProvider
    {
        /// <inheritdoc />
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            if (context.Metadata.ModelType == typeof(CancellationToken)) return new CancellationTokenModelBinder();

            return null;
        }
    }
}