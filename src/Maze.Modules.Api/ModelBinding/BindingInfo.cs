// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Maze.Modules.Api.Parameters;

namespace Maze.Modules.Api.ModelBinding
{
    /// <summary>
    ///     Binding info which represents metadata associated to an action parameter.
    /// </summary>
    public class BindingInfo
    {
        /// <summary>
        ///     Gets or sets the <see cref="BindingSource" />.
        /// </summary>
        public BindingSource BindingSource { get; set; }

        /// <summary>
        ///     Gets or sets the binder model name.
        /// </summary>
        public string BinderModelName { get; set; }
    }
}