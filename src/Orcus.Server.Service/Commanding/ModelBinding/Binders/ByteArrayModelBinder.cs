using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Orcus.Server.Service.Commanding.Binders;

namespace Orcus.Server.Service.Commanding.ModelBinding.Binders
{
    /// <summary>
    /// ModelBinder to bind byte Arrays.
    /// </summary>
    public class ByteArrayModelBinder : IModelBinder
    {
        private readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of <see cref="ByteArrayModelBinder"/>.
        /// </summary>
        /// <param name="loggerFactory">The <see cref="ILoggerFactory"/>.</param>
        public ByteArrayModelBinder(ILoggerFactory loggerFactory)
        {
            if (loggerFactory == null)
            {
                throw new ArgumentNullException(nameof(loggerFactory));
            }

            _logger = loggerFactory.CreateLogger<ByteArrayModelBinder>();
        }

        /// <inheritdoc />
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
                throw new ArgumentNullException(nameof(bindingContext));

            _logger.LogDebug("Attempting to bind to model");

            // Check for missing data case 1: There was no <input ... /> element containing this data.
            var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            if (valueProviderResult == ValueProviderResult.None)
            {
                _logger.LogWarning($"Value {bindingContext.ModelName} not found in request");
                return Task.CompletedTask;
            }
            
            // Check for missing data case 2: There was an <input ... /> element but it was left blank.
            var value = valueProviderResult.FirstValue;
            if (string.IsNullOrEmpty(value))
            {
                _logger.LogWarning($"Value {bindingContext.ModelName} not found in request");
                return Task.CompletedTask;
            }

            try
            {
                var model = Convert.FromBase64String(value);
                bindingContext.Result = ModelBindingResult.Success(model);
            }
            catch (Exception exception)
            {
                _logger.LogWarning(exception, "Exception occurred when trying to read base 64 string");
            }
            
            _logger.LogDebug("Done");
            return Task.CompletedTask;
        }
    }
}
