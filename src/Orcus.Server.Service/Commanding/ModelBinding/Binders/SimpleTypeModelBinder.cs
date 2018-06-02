using System;
using System.ComponentModel;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Orcus.Server.Service.Commanding.Binders;

namespace Orcus.Server.Service.Commanding.ModelBinding.Binders
{
    /// <summary>
    ///     An <see cref="IModelBinder" /> for simple types.
    /// </summary>
    public class SimpleTypeModelBinder : LoggingBinderBase, IModelBinder
    {
        private readonly TypeConverter _typeConverter;

        /// <summary>
        ///     Initializes a new instance of <see cref="SimpleTypeModelBinder" />.
        /// </summary>
        /// <param name="type">The type to create binder for.</param>
        /// <param name="loggerFactory">The <see cref="ILoggerFactory" />.</param>
        public SimpleTypeModelBinder(Type type, ILoggerFactory loggerFactory) : base(
            loggerFactory.CreateLogger<SimpleTypeModelBinder>())
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            if (loggerFactory == null)
                throw new ArgumentNullException(nameof(loggerFactory));

            _typeConverter = TypeDescriptor.GetConverter(type);
        }

        /// <inheritdoc />
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
                throw new ArgumentNullException(nameof(bindingContext));

            var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            if (valueProviderResult == ValueProviderResult.None)
            {
                LogFoundNoValueInRequest(bindingContext);

                // no entry
                LogDoneAttemptingToBindModel(bindingContext);
                return Task.CompletedTask;
            }

            LogAttemptingToBindModel(bindingContext);

            try
            {
                var value = valueProviderResult.FirstValue;

                object model;
                if (bindingContext.ModelType == typeof(string))
                {
                    // Already have a string. No further conversion required but handle ConvertEmptyStringToNull.
                    if (bindingContext.ModelMetadata.ConvertEmptyStringToNull && string.IsNullOrWhiteSpace(value))
                        model = null;
                    else
                        model = value;
                }
                else if (string.IsNullOrWhiteSpace(value))
                {
                    // Other than the StringConverter, converters Trim() the value then throw if the result is empty.
                    model = null;
                }
                else
                {
                    model = _typeConverter.ConvertFrom(
                        null,
                        valueProviderResult.Culture,
                        value);
                }

                CheckModel(bindingContext, valueProviderResult, model);

                LogDoneAttemptingToBindModel(bindingContext);
                return Task.CompletedTask;
            }
            catch (Exception exception)
            {
                var isFormatException = exception is FormatException;
                if (!isFormatException && exception.InnerException != null)
                    exception = ExceptionDispatchInfo.Capture(exception.InnerException).SourceException;

                LogException(bindingContext, exception);
                bindingContext.ModelState.AddError(exception, bindingContext.ModelMetadata);

                // Were able to find a converter for the type but conversion failed.
                return Task.CompletedTask;
            }
        }

        protected virtual void CheckModel(
            ModelBindingContext bindingContext,
            ValueProviderResult valueProviderResult,
            object model)
        {
            // When converting newModel a null value may indicate a failed conversion for an otherwise required
            // model (can't set a ValueType to null). This detects if a null model value is acceptable given the
            // current bindingContext. If not, an error is logged.
            if (model == null && !bindingContext.ModelMetadata.IsReferenceOrNullableType)
                bindingContext.ModelState.AddError(new InvalidOperationException("Value must not be null."),
                    bindingContext.ModelMetadata);
            else
                bindingContext.Result = ModelBindingResult.Success(model);
        }
    }
}