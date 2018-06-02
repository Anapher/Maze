using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Orcus.Server.Service.Commanding.Binders;

namespace Orcus.Server.Service.Commanding.ModelBinding.Binders
{
    /// <summary>
    ///     An <see cref="IModelBinderProvider" /> for binding simple data types.
    /// </summary>
    public class SimpleTypeModelBinderProvider : IModelBinderProvider
    {
        /// <inheritdoc />
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            if (!context.Metadata.IsComplexType)
            {
                var loggerFactory = context.Services.GetRequiredService<ILoggerFactory>();
                return new SimpleTypeModelBinder(context.Metadata.ModelType, loggerFactory);
            }

            return null;
        }
    }
}