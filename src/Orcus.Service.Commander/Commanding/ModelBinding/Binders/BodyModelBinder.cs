// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Orcus.Modules.Api.Parameters;
using Orcus.Server.Service.Commanding.Formatters;
using Orcus.Server.Service.Commanding.Formatters.Abstractions;
using Orcus.Server.Service.Infrastructure;

namespace Orcus.Server.Service.Commanding.ModelBinding.Binders
{
    /// <summary>
    ///     An <see cref="IModelBinder" /> which binds models from the request body using an <see cref="IInputFormatter" />
    ///     when a model has the binding source <see cref="BindingSource.Body" />.
    /// </summary>
    public class BodyModelBinder : LoggingBinderBase, IModelBinder
    {
        private readonly IList<IInputFormatter> _formatters;
        private readonly Func<Stream, Encoding, TextReader> _readerFactory;
        private const bool AllowEmptyInputInBodyModelBinding = true;

        /// <summary>
        ///     Creates a new <see cref="BodyModelBinder" />.
        /// </summary>
        /// <param name="formatters">The list of <see cref="IInputFormatter" />.</param>
        /// <param name="readerFactory">
        ///     The <see cref="IHttpRequestStreamReaderFactory" />, used to create <see cref="System.IO.TextReader" />
        ///     instances for reading the request body.
        /// </param>
        /// <param name="loggerFactory">The <see cref="ILoggerFactory" />.</param>
        public BodyModelBinder(
            IList<IInputFormatter> formatters,
            IHttpRequestStreamReaderFactory readerFactory,
            ILoggerFactory loggerFactory) : base(loggerFactory.CreateLogger<BodyModelBinder>())
        {
            if (readerFactory == null) throw new ArgumentNullException(nameof(readerFactory));

            _formatters = formatters ?? throw new ArgumentNullException(nameof(formatters));
            _readerFactory = readerFactory.CreateReader;
        }

        /// <inheritdoc />
        public async Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null) throw new ArgumentNullException(nameof(bindingContext));

            LogAttemptingToBindModel(bindingContext);

            // Special logic for body, treat the model name as string.Empty for the top level
            // object, but allow an override via BinderModelName. The purpose of this is to try
            // and be similar to the behavior for POCOs bound via traditional model binding.
            var modelBindingKey = bindingContext.ModelName;

            var httpContext = bindingContext.OrcusContext;

            var formatterContext = new InputFormatterContext(
                httpContext,
                modelBindingKey,
                bindingContext.ModelState,
                bindingContext.ModelMetadata,
                _readerFactory,
                AllowEmptyInputInBodyModelBinding);

            var formatter = (IInputFormatter) null;
            for (var i = 0; i < _formatters.Count; i++)
                if (_formatters[i].CanRead(formatterContext))
                {
                    formatter = _formatters[i];

                    LogInputFormatterSelected(formatter, formatterContext);
                    break;
                }
                else
                {
                    LogInputFormatterRejected(_formatters[i], formatterContext);
                }

            if (formatter == null)
            {
                LogNoInputFormatterSelected(formatterContext);
                
                var exception = new ArgumentException($"Unsupported content type: {httpContext.Request.ContentType}");
                bindingContext.ModelState.AddError(exception, bindingContext.ModelMetadata);
                LogDoneAttemptingToBindModel(bindingContext);
                return;
            }

            try
            {
                var result = await formatter.ReadAsync(formatterContext);

                if (result.HasError)
                {
                    // Formatter encountered an error. Do not use the model it returned.
                    LogDoneAttemptingToBindModel(bindingContext);
                    return;
                }

                if (result.IsModelSet)
                {
                    var model = result.Model;
                    bindingContext.Result = ModelBindingResult.Success(model);
                }
                else
                {
                    // If the input formatter gives a "no value" result, that's always a model state error,
                    // because BodyModelBinder implicitly regards input as being required for model binding.
                    // If instead the input formatter wants to treat the input as optional, it must do so by
                    // returning InputFormatterResult.Success(defaultForModelType), because input formatters
                    // are responsible for choosing a default value for the model type.
                    bindingContext.ModelState.AddError(new ArgumentException("MissingRequestBodyRequiredValueAccessor"),
                        bindingContext.ModelMetadata);
                }
            }
            catch (Exception exception)
            {
                bindingContext.ModelState.AddError(exception, bindingContext.ModelMetadata);
                //bindingContext.ModelState.AddModelError(modelBindingKey, exception, bindingContext.ModelMetadata);
            }

            LogDoneAttemptingToBindModel(bindingContext);
        }
    }
}