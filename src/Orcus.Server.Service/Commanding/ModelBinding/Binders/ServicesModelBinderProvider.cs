using System;
using Orcus.Modules.Api.Parameters;
using Orcus.Server.Service.Commanding.Binders;

namespace Orcus.Server.Service.Commanding.ModelBinding.Binders
{
    /// <summary>
    ///     An <see cref="IModelBinderProvider" /> for binding from the <see cref="IServiceProvider" />.
    /// </summary>
    public class ServicesModelBinderProvider : IModelBinderProvider
    {
        /// <inheritdoc />
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            if (context.Metadata.BindingSource == BindingSource.Services)
                return new ServicesModelBinder();

            return null;
        }
    }
}