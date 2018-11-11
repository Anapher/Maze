using System;
using System.Collections.Generic;
using System.Text;
using Orcus.Server.Connection;
using Orcus.Server.Connection.Error;

namespace Tasks.Infrastructure.Core
{
    public enum TaskErrorCode
    {
        Tasks_NotFound = 116 * 97 * 115 * 107, //ascii for "task"
        Tasks_TaskIdNull,
        Tasks_RestartOnFailIntervalMustBePositive,
        Tasks_MaximumRestartsMustBeGreaterThanZero,
        Tasks_NoAudienceGiven,
        Tasks_NoTriggersGiven,
        Tasks_NoCommandsGiven,
    }

    public static class TaskErrors
    {
        public static RestError TaskNotFound => CreateNotFoundError("The task was not found.", TaskErrorCode.Tasks_NotFound);
        public static RestError TaskGuidEmpty => CreateValidationError("The guid of the task may not be null.", TaskErrorCode.Tasks_TaskIdNull);
        public static RestError RestartOnFailIntervalMustBePositive => CreateValidationError("The restart on fail interval must be a positive timespan.", TaskErrorCode.Tasks_RestartOnFailIntervalMustBePositive);
        public static RestError MaximumRestartsMustBeGreaterThanZero => CreateValidationError("The maximum amount of restarts must be greater than zero.", TaskErrorCode.Tasks_MaximumRestartsMustBeGreaterThanZero);
        public static RestError NoAudienceGiven => CreateValidationError("No audience was given.", TaskErrorCode.Tasks_NoAudienceGiven);
        public static RestError NoTriggersGiven => CreateValidationError("No triggers were given.", TaskErrorCode.Tasks_NoTriggersGiven);
        public static RestError NoCommandsGiven => CreateValidationError("No commands were given.", TaskErrorCode.Tasks_NoCommandsGiven);

        private static RestError CreateValidationError(string message, TaskErrorCode code) =>
            new RestError(ErrorTypes.ValidationError, message, (int)code);

        private static RestError CreateInvalidOperationError(string message, TaskErrorCode code) =>
            new RestError(ErrorTypes.InvalidOperationError, message, (int)code);

        private static RestError CreateNotFoundError(string message, TaskErrorCode code) =>
            new RestError(ErrorTypes.NotFoundError, message, (int)code);
    }
}
