using System.Buffers;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.ObjectPool;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Maze.Modules.Api.Formatters;
using Maze.Service.Commander.Commanding.Formatters.Abstractions;
using Maze.Service.Commander.Commanding.Formatters.Json;
using Maze.Service.Commander.Commanding.ModelBinding;
using Maze.Service.Commander.Commanding.ModelBinding.Binders;
using Maze.Service.Commander.Infrastructure;

namespace Maze.Service.Commander
{
    public class MazeServerOptions
    {
        public MazeServerOptions()
        {
            var loggerFactory = NullLoggerFactory.Instance;

            var charPool = ArrayPool<char>.Shared;
            var objectPool = new DefaultObjectPoolProvider();

            InputFormatters = new Collection<IInputFormatter>
            {
                new JsonInputFormatter(loggerFactory.CreateLogger<JsonInputFormatter>(), SerializerSettings, charPool,
                    objectPool),
                new JsonPatchInputFormatter(loggerFactory.CreateLogger<JsonPatchInputFormatter>(), SerializerSettings,
                    charPool, objectPool)
            };

            OutputFormatters = new List<IOutputFormatter>
            {
                new JsonOutputFormatter(SerializerSettings, charPool)
            };

            ModelBinderProviders = new List<IModelBinderProvider>
            {
                new ServicesModelBinderProvider(),
                new BodyModelBinderProvider(InputFormatters,
                    new MemoryPoolHttpRequestStreamReaderFactory(ArrayPool<byte>.Shared, charPool)),
                new EnumTypeModelBinderProvider(),
                new SimpleTypeModelBinderProvider(),
                new ByteArrayModelBinderProvider(),
                new CancellationTokenModelBinderProvider()
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

        public JsonSerializerSettings SerializerSettings { get; set; } = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            NullValueHandling = NullValueHandling.Ignore
        };
    }
}