// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Orcus.Service.Commander.Commanding.ModelBinding.Binders
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