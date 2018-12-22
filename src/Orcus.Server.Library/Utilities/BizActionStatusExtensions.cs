using System.Linq;
using System.Net;
using CodeElements.BizRunner;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Orcus.Server.Connection;
using Orcus.Server.Connection.Error;

namespace Orcus.Server.Library.Utilities
{
    /// <summary>
    ///     Extensions for business actions
    /// </summary>
    public static class BizActionStatusExtensions
    {
        /// <summary>
        ///     On succeed, returns an <see cref="OkResult"/>, else return a failed response with the error messages.
        /// </summary>
        /// <param name="status">The business action status</param>
        public static IActionResult ToActionResult(this IBizActionStatus status)
        {
            if (!status.HasErrors)
                return new OkResult();

            var errors = status.Errors.OfType<RestErrorValidationResult>().Select(x => x.Error).ToArray();
            var firstError = errors.FirstOrDefault();
            var httpCode = firstError == null ? StatusCodes.Status400BadRequest : (int) ErrorTypes.ErrorStatusCodes[firstError.Type];
            return new JsonResult(errors) {StatusCode = httpCode};
        }

        /// <summary>
        ///     Create an action result out of a rest error
        /// </summary>
        /// <param name="error">The <see cref="RestError"/></param>
        public static IActionResult ToActionResult(this RestError error)
        {
            var httpCode = (int) ErrorTypes.ErrorStatusCodes[error.Type];
            return new JsonResult(new[] {error}) {StatusCode = httpCode};
        }
    }
}