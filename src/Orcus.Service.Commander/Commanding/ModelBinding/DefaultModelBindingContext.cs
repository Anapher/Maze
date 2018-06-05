// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Orcus.Modules.Api;
using Orcus.Modules.Api.ModelBinding;

namespace Orcus.Server.Service.Commanding.ModelBinding
{
    public class DefaultModelBindingContext : ModelBindingContext
    {
        /// <inheritdoc />
        public override ActionContext ActionContext { get; set; }

        /// <inheritdoc />
        public override object Model { get; set; }

        /// <inheritdoc />
        public override IValueProvider ValueProvider { get; set; }

        /// <inheritdoc />
        public override string ModelName { get; set; }

        /// <inheritdoc />
        public override ModelBindingResult Result { get; set; }

        /// <inheritdoc />
        public override ModelMetadata ModelMetadata { get; set; }

        public override ModelStateDictionary ModelState { get; set; }

        /// <summary>
        ///     Creates a new <see cref="DefaultModelBindingContext" /> for top-level model binding operation.
        /// </summary>
        /// <param name="actionContext">
        ///     The <see cref="ActionContext" /> associated with the binding operation.
        /// </param>
        /// <param name="valueProvider">The <see cref="IValueProvider" /> to use for binding.</param>
        /// <param name="metadata"><see cref="ModelMetadata" /> associated with the model.</param>
        /// <param name="modelName">The name of the property or parameter being bound.</param>
        /// <returns>A new instance of <see cref="DefaultModelBindingContext" />.</returns>
        public static ModelBindingContext CreateBindingContext(
            ActionContext actionContext,
            IValueProvider valueProvider,
            ModelMetadata metadata,
            string modelName)
        {
            if (actionContext == null)
                throw new ArgumentNullException(nameof(actionContext));

            if (valueProvider == null)
                throw new ArgumentNullException(nameof(valueProvider));

            if (metadata == null)
                throw new ArgumentNullException(nameof(metadata));

            if (modelName == null)
                throw new ArgumentNullException(nameof(modelName));

            var binderModelName = metadata.BinderModelName;

            return new DefaultModelBindingContext
            {
                ActionContext = actionContext,

                // Because this is the top-level context, FieldName and ModelName should be the same.
                ModelName = binderModelName ?? modelName,

                ModelMetadata = metadata,
                ValueProvider = valueProvider,
                ModelState = null //TODO
            };
        }
    }
}