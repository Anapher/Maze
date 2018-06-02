using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Orcus.Server.Service.Commanding.Binders;

namespace Orcus.Server.Service.Commanding.ModelBinding.Binders
{
    /// <summary>
    ///     A <see cref="IModelBinderProvider" /> for types deriving from <see cref="Enum" />.
    /// </summary>
    public class EnumTypeModelBinderProvider : IModelBinderProvider
    {
        private const bool SuppressBindingUndefinedValueToEnumType = true;

        /// <inheritdoc />
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            if (context.Metadata.IsEnum)
            {
                var loggerFactory = context.Services.GetRequiredService<ILoggerFactory>();
                return new EnumTypeModelBinder(
                    SuppressBindingUndefinedValueToEnumType,
                    context.Metadata.UnderlyingOrModelType,
                    loggerFactory);
            }

            return null;
        }
    }
}