using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Orcus.Modules.Api.Parameters;
using Orcus.Server.Service.Commanding.Binders;

namespace Orcus.Server.Service.Commanding.ModelBinding.Binders
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

            var requestServices = bindingContext.OrcusContext.ServiceProvider;
            var model = requestServices.GetRequiredService(bindingContext.ModelType);

            bindingContext.Result = ModelBindingResult.Success(model);
            return Task.CompletedTask;
        }
    }
}