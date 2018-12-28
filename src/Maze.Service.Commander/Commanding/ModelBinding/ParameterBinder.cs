// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading.Tasks;
using Maze.Modules.Api;
using Maze.Modules.Api.ModelBinding;

namespace Maze.Service.Commander.Commanding.ModelBinding
{
    public class ParameterBinder
    {
        /// <summary>
        ///     Binds a model specified by <paramref name="parameter" /> using <paramref name="value" /> as the initial value.
        /// </summary>
        /// <param name="actionContext">The <see cref="ActionContext" />.</param>
        /// <param name="modelBinder">The <see cref="IModelBinder" />.</param>
        /// <param name="valueProvider">The <see cref="IValueProvider" />.</param>
        /// <param name="parameter">The <see cref="ParameterDescriptor" /></param>
        /// <param name="metadata">The <see cref="ModelMetadata" />.</param>
        /// <param name="value">The initial model value.</param>
        /// <returns>The result of model binding.</returns>
        public virtual async Task<ModelBindingResult> BindModelAsync(ActionContext actionContext,
            IModelBinder modelBinder, IValueProvider valueProvider, ParameterDescriptor parameter,
            ModelMetadata metadata, object value)
        {
            if (actionContext == null)
                throw new ArgumentNullException(nameof(actionContext));

            if (modelBinder == null)
                throw new ArgumentNullException(nameof(modelBinder));

            if (valueProvider == null)
                throw new ArgumentNullException(nameof(valueProvider));

            if (parameter == null)
                throw new ArgumentNullException(nameof(parameter));

            if (metadata == null)
                throw new ArgumentNullException(nameof(metadata));

            var modelBindingContext =
                DefaultModelBindingContext.CreateBindingContext(actionContext, valueProvider, metadata, parameter.Name);
            modelBindingContext.Model = value;
            modelBindingContext.ModelState = new ModelStateDictionary();

            var parameterModelName = metadata.BinderModelName;
            if (parameterModelName != null)
                modelBindingContext.ModelName = parameterModelName;
            else if (modelBindingContext.ValueProvider.ContainsPrefix(parameter.Name))
                modelBindingContext.ModelName = parameter.Name;
            else
                modelBindingContext.ModelName = string.Empty;

            await modelBinder.BindModelAsync(modelBindingContext);
            return modelBindingContext.Result;
        }
    }
}