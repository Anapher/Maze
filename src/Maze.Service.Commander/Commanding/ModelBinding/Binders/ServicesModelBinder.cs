// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Maze.Modules.Api.Parameters;
using Maze.Service.Commander.Commanding.ModelBinding.Abstract;

namespace Maze.Service.Commander.Commanding.ModelBinding.Binders
{
    /// <summary>
    ///     An <see cref="IModelBinder" /> which binds models from the request services when a model
    ///     has the binding source <see cref="BindingSource.Services" />/
    /// </summary>
    public class ServicesModelBinder : IModelBinder
    {
        /// <inheritdoc />
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null) throw new ArgumentNullException(nameof(bindingContext));

            var requestServices = bindingContext.MazeContext.RequestServices;
            var model = requestServices.GetRequiredService(bindingContext.ModelType);

            bindingContext.Result = ModelBindingResult.Success(model);
            return Task.CompletedTask;
        }
    }
}