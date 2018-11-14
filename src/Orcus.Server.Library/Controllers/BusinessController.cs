using System;
using System.Threading.Tasks;
using CodeElements.BizRunner;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Orcus.Server.Connection;
using Orcus.Server.Connection.Error;
using Orcus.Server.Library.Utilities;

namespace Orcus.Server.Library.Controllers
{
    /// <summary>
    ///     A <see cref="Controller"/> that provides methods to response <see cref="IBizActionStatus"/>.
    /// </summary>
    [ApiController, Authorize]
    public abstract class BusinessController : Controller
    {
        /// <summary>
        ///     Response a rest error
        /// </summary>
        /// <param name="restError">The rest error that should be responsed</param>
        [NonAction]
        public IActionResult RestError(RestError restError)
        {
            var httpCode = (int) ErrorTypes.ErrorStatusCodes[restError.Type];
            return StatusCode(httpCode, new[] {restError});
        }

        /// <summary>
        ///     Response a business action status
        /// </summary>
        /// <param name="status">The status to response</param>
        [NonAction]
        public IActionResult BizActionStatus(IBizActionStatus status)
        {
            return status.ToActionResult();
        }

        /// <summary>
        ///     Response the errors of a business action or call a delegate to get a result
        /// </summary>
        /// <param name="status">The business action status</param>
        /// <param name="getNormalActionResult">The delegate that will be invoked in case the <see cref="status"/> succeeded.</param>
        [NonAction]
        public IActionResult BizActionStatus(IBizActionStatus status, Func<IActionResult> getNormalActionResult)
        {
            if (status.HasErrors)
                return status.ToActionResult();

            return getNormalActionResult();
        }

        /// <summary>
        ///     Response the errors of a business action or call an async delegate to get a result
        /// </summary>
        /// <param name="status">The business action status</param>
        /// <param name="getNormalActionResult">The delegate that will be invoked in case the <see cref="status"/> succeeded.</param>
        [NonAction]
        public Task<IActionResult> BizActionStatus(IBizActionStatus status, Func<Task<IActionResult>> getNormalActionResult)
        {
            if (status.HasErrors)
                return Task.FromResult(status.ToActionResult());

            return getNormalActionResult();
        }
    }
}