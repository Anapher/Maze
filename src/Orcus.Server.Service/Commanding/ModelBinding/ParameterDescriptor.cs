using System;
using Orcus.Modules.Api.Parameters;

namespace Orcus.Server.Service.Commanding.ModelBinding
{
    public class ParameterDescriptor
    {
        public string Name { get; set; }
        public Type ParameterType { get; set; }

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