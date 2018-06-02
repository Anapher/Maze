using System;
using Microsoft.Extensions.Logging;
using Orcus.Server.Service.Commanding.Formatters;

namespace Orcus.Server.Service.Commanding.ModelBinding
{
    public abstract class LoggingBinderBase
    {
        protected readonly ILogger Logger;

        protected LoggingBinderBase(ILogger logger)
        {
            Logger = logger;
        }

        protected void LogFoundNoValueInRequest(ModelBindingContext context)
        {
        }

        protected void LogDoneAttemptingToBindModel(ModelBindingContext context)
        {
        }

        protected void LogAttemptingToBindModel(ModelBindingContext context)
        {
        }

        protected void LogException(ModelBindingContext context, Exception exception)
        {
        }

        protected void LogInputFormatterSelected(IInputFormatter formatter, InputFormatterContext formatterContext)
        {
        }

        protected void LogInputFormatterRejected(IInputFormatter formatter, InputFormatterContext formatterContext)
        {

        }

        protected void LogNoInputFormatterSelected(InputFormatterContext formatterContext)
        {

        }
    }
}