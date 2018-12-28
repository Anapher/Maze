// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Maze.Modules.Api.Parameters;

namespace Maze.Service.Commander.Commanding.ModelBinding.Binders
{
    /// <summary>
    ///     An <see cref="IModelBinderProvider" /> for binding from the <see cref="IServiceProvider" />.
    /// </summary>
    public class ServicesModelBinderProvider : IModelBinderProvider
    {
        /// <inheritdoc />
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            if (context.Metadata.BindingSource == BindingSource.Services)
                return new ServicesModelBinder();

            return null;
        }
    }
}