// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.ComponentModel;
using System.Reflection;
using Orcus.Modules.Api.Parameters;

namespace Orcus.Modules.Api.ModelBinding
{
    /// <summary>
    ///     A metadata representation of a model type, property or parameter.
    /// </summary>
    public class ModelMetadata
    {
        public ModelMetadata(ParameterDescriptor parameterDescriptor)
        {
            Name = parameterDescriptor.Name;
            ModelType = parameterDescriptor.ParameterType;
            BinderModelName = parameterDescriptor.BindingInfo.BinderModelName;
            BindingSource = parameterDescriptor.BindingInfo.BindingSource;

            InitializeTypeInformation();
        }

        /// <summary>
        ///     Gets the model type represented by the current instance.
        /// </summary>
        public Type ModelType { get; set; }

        /// <summary>
        ///     Gets the underlying type argument if <see cref="ModelType" /> inherits from <see cref="Nullable{T}" />.
        ///     Otherwise gets <see cref="ModelType" />.
        /// </summary>
        /// <remarks>
        ///     Identical to <see cref="ModelType" /> unless <see cref="IsNullableValueType" /> is <c>true</c>.
        /// </remarks>
        public Type UnderlyingOrModelType { get; private set; }

        /// <summary>
        ///     Gets the name of the parameter
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Gets a binder metadata for this model.
        /// </summary>
        public BindingSource BindingSource { get; }

        /// <summary>
        ///     Gets a value indicating whether <see cref="ModelType" /> is a complex type.
        /// </summary>
        /// <remarks>
        ///     A complex type is defined as a <see cref="Type" /> which has a
        ///     <see cref="TypeConverter" /> that can convert from <see cref="string" />.
        /// </remarks>
        public bool IsComplexType { get; private set; }

        /// <summary>
        ///     Gets a value indicating whether or not <see cref="ModelType" /> allows <c>null</c> values.
        /// </summary>
        public bool IsReferenceOrNullableType { get; private set; }

        /// <summary>
        ///     Gets a value indicating whether or not <see cref="ModelType" /> is a <see cref="Nullable{T}" />.
        /// </summary>
        public bool IsNullableValueType { get; private set; }

        /// <summary>
        ///     Gets the name of a model if specified explicitly using <see cref="IModelNameProvider" />.
        /// </summary>
        public string BinderModelName { get; }

        public bool ConvertEmptyStringToNull { get; set; }

        public bool IsEnum { get; private set; }

        public bool IsFlagsEnum { get; set; }

        private void InitializeTypeInformation()
        {
            UnderlyingOrModelType = Nullable.GetUnderlyingType(ModelType) ?? ModelType;
            IsComplexType = !TypeDescriptor.GetConverter(ModelType).CanConvertFrom(typeof(string));
            IsNullableValueType = Nullable.GetUnderlyingType(ModelType) != null;
            IsReferenceOrNullableType = !ModelType.GetTypeInfo().IsValueType || IsNullableValueType;
            IsEnum = ModelType.IsEnum;
            IsFlagsEnum = ModelType.IsDefined(typeof(FlagsAttribute), false);
        }
    }
}