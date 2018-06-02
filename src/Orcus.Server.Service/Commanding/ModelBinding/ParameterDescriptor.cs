using System;
using Orcus.Modules.Api.Parameters;

namespace Orcus.Server.Service.Commanding.ModelBinding
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