using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Orcus.Modules.Api.Formatters;
using Orcus.Server.Service.Commanding.ModelBinding;
using Orcus.Server.Service.Commanding.ModelBinding.Binders;
using Orcus.Server.Service.Infrastructure;

namespace Orcus.Server.Service
{
    public class OrcusServerOptions
    {
        public OrcusServerOptions()
        {
            ModelBinderProviders = new List<IModelBinderProvider>
            {
                new ServicesModelBinderProvider(),
                new BodyModelBinderProvider(formatters, new MemoryPoolHttpRequestStreamReaderFactory()),
                new EnumTypeModelBinderProvider(),
                new SimpleTypeModelBinderProvider(),
                new ByteArrayModelBinderProvider()
            };
        }

        public IList<IOutputFormatter> OutputFormatters { get; }

        /// <summary>
        /// Gets or sets the flag which causes content negotiation to ignore Accept header
        /// when it contains the media type */*. <see langword="false"/> by default.
        /// </summary>
        public bool RespectBrowserAcceptHeader { get; set; }

        /// <summary>
        /// Gets or sets the flag which decides whether an HTTP 406 Not Acceptable response
        /// will be returned if no formatter has been selected to format the response.
        /// <see langword="false"/> by default.
        /// </summary>
        public bool ReturnHttpNotAcceptable { get; set; }

        /// <summary>
        /// Gets a list of <see cref="IModelBinderProvider"/>s used by this application.
        /// </summary>
        public IList<IModelBinderProvider> ModelBinderProviders { get; }
    }

    public static class OrcusServerServices
    {


        public static IServiceCollection RegisterOrcusServices(this IServiceCollection serviceCollection)
        {
        }
    }
}