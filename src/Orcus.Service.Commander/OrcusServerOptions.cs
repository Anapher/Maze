using System.Buffers;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Orcus.Modules.Api.Formatters;
using Orcus.Service.Commander.Commanding.Formatters.Abstractions;
using Orcus.Service.Commander.Commanding.ModelBinding;
using Orcus.Service.Commander.Commanding.ModelBinding.Binders;
using Orcus.Service.Commander.Infrastructure;

namespace Orcus.Service.Commander
{
    public class OrcusServerOptions
    {
        public OrcusServerOptions()
        {
            InputFormatters = new Collection<IInputFormatter>();

            ModelBinderProviders = new List<IModelBinderProvider>
            {
                new ServicesModelBinderProvider(),
                new BodyModelBinderProvider(InputFormatters,
                    new MemoryPoolHttpRequestStreamReaderFactory(ArrayPool<byte>.Shared, ArrayPool<char>.Shared)),
                new EnumTypeModelBinderProvider(),
                new SimpleTypeModelBinderProvider(),
                new ByteArrayModelBinderProvider()
            };
        }

        /// <summary>
        /// Gets a list of <see cref="IInputFormatter"/>s that are used by this application.
        /// </summary>
        public Collection<IInputFormatter> InputFormatters { get; }

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
}