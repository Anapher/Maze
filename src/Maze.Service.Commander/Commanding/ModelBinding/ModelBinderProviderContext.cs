// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Maze.Modules.Api.ModelBinding;

namespace Maze.Service.Commander.Commanding.ModelBinding
{
    public abstract class ModelBinderProviderContext
    {
        /// <summary>
        ///     Gets the <see cref="ModelMetadata" />.
        /// </summary>
        public abstract ModelMetadata Metadata { get; }

        /// <summary>
        ///     Gets the <see cref="IServiceProvider" />.
        /// </summary>
        public abstract IServiceProvider Services { get; }
    }
}