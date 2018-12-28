using System.ComponentModel.DataAnnotations;
using CodeElements.BizRunner;
using Microsoft.Extensions.Logging;

namespace Maze.Server.BusinessLogic
{
    public abstract class LoggingBusinessActionErrors : BusinessActionErrors
    {
        protected readonly ILogger Logger;

        protected LoggingBusinessActionErrors(ILogger logger)
        {
            Logger = logger;
        }

        protected override bool ValidateModelFailed(IValidatableObject model)
        {
            Logger.LogDebug("Validating model");
            Logger.LogTrace("Model: {@model}", model);
            var result = base.ValidateModelFailed(model);
            Logger.LogDebug("Validation completed, HasErrors: {hasErrors}", result);

            return result;
        }

        protected override T ReturnError<T>(ValidationResult validationResult)
        {
            Logger.LogTrace("Return custom error '{@error}'", validationResult);
            return base.ReturnError<T>(validationResult);
        }
    }
}