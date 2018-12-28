// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Maze.Modules.Api.Parameters;
using Maze.Service.Commander.Commanding.Formatters.Abstractions;
using Maze.Service.Commander.Infrastructure;

namespace Maze.Service.Commander.Commanding.ModelBinding.Binders
{
   public class BodyModelBinderProvider : IModelBinderProvider
    {
        private readonly IList<IInputFormatter> _formatters;
        private readonly IHttpRequestStreamReaderFactory _readerFactory;

        public BodyModelBinderProvider(IList<IInputFormatter> formatters, IHttpRequestStreamReaderFactory readerFactory)
        {
            _formatters = formatters ?? throw new ArgumentNullException(nameof(formatters));
            _readerFactory = readerFactory ?? throw new ArgumentNullException(nameof(readerFactory));
        }

        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            if (context.Metadata.BindingSource == BindingSource.Body)
            {
                if (_formatters.Count == 0)
                    throw new InvalidOperationException("No formatters found.");

                var loggerFactory = context.Services.GetRequiredService<ILoggerFactory>();
                return new BodyModelBinder(_formatters, _readerFactory, loggerFactory);
            }

            return null;
        }
    }
}
