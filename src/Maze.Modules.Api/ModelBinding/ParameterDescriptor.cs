// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Orcus.Modules.Api.ModelBinding
{
    public class ParameterDescriptor
    {
        /// <summary>
        ///     The name of the parameter variable
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     The type of the parameter that should be bound
        /// </summary>
        public Type ParameterType { get; set; }

        /// <summary>
        ///     Binding info which represents metadata associated to an action parameter.
        /// </summary>
        public BindingInfo BindingInfo { get; set; }
    }
}