using System.Linq;
using CodeElements.BizRunner;
using Microsoft.AspNetCore.Mvc;
using Orcus.Server.Connection;
using Orcus.Server.Connection.Error;

namespace Orcus.Server.Library.Utilities
{
    public static class BizActionStatusExtensions
    {
        public static IActionResult ToActionResult(this IBizActionStatus status)
        {
            if (!status.HasErrors)
                return new OkResult();

            var errors = status.Errors.OfType<RestErrorValidationResult>().Select(x => x.Error).ToArray();
            var firstError = errors[0];
            var httpCode = (int) ErrorTypes.ErrorStatusCodes[firstError.Type];
            return new JsonResult(errors) {StatusCode = httpCode};
        }

        public static IActionResult ToActionResult(this RestError error)
        {
            var httpCode = (int) ErrorTypes.ErrorStatusCodes[error.Type];
            return new JsonResult(new[] {error}) {StatusCode = httpCode};
        }
    }
}