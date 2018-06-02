using System;
using System.ComponentModel;
using System.Reflection;
using Orcus.Modules.Api.Parameters;

namespace Orcus.Server.Service.Commanding.ModelBinding
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
            BinderModelName = parameterDescriptor.BinderModelName;
            BindingSource = parameterDescriptor.BindingSource;

            InitializeTypeInformation();
        }

        public Type ModelType { get; set; }

        /// <summary>
        /// Gets the name of the parameter
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets a binder metadata for this model.
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
        /// Gets a value indicating whether or not <see cref="ModelType"/> allows <c>null</c> values.
        /// </summary>
        public bool IsReferenceOrNullableType { get; private set; }

        /// <summary>
        /// Gets a value indicating whether or not <see cref="ModelType"/> is a <see cref="Nullable{T}"/>.
        /// </summary>
        public bool IsNullableValueType { get; private set; }

        /// <summary>
        /// Gets the name of a model if specified explicitly using <see cref="IModelNameProvider"/>.
        /// </summary>
        public string BinderModelName { get; }

        private void InitializeTypeInformation()
        {
            IsComplexType = !TypeDescriptor.GetConverter(ModelType).CanConvertFrom(typeof(string));
            IsNullableValueType = Nullable.GetUnderlyingType(ModelType) != null;
            IsReferenceOrNullableType = !ModelType.GetTypeInfo().IsValueType || IsNullableValueType;
        }
    }
}