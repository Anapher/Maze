using System;
using System.Threading.Tasks;
using CodeElements.BizRunner;
using Microsoft.AspNetCore.Mvc;
using Orcus.Server.Connection;
using Orcus.Server.Connection.Error;
using Orcus.Server.Utilities;

namespace Orcus.Server.ControllersBase
{
    public abstract class BusinessController : Controller
    {
        [NonAction]
        public IActionResult RestError(RestError restError)
        {
            var httpCode = (int) ErrorTypes.ErrorStatusCodes[restError.Type];
            return StatusCode(httpCode, new[] {restError});
        }

        [NonAction]
        public IActionResult BizActionStatus(IBizActionStatus status)
        {
            return status.ToActionResult();
        }

        [NonAction]
        public IActionResult BizActionStatus(IBizActionStatus status, Func<IActionResult> getNormalActionResult)
        {
            if (status.HasErrors)
                return status.ToActionResult();

            return getNormalActionResult();
        }

        [NonAction]
        public Task<IActionResult> BizActionStatus(IBizActionStatus status, Func<Task<IActionResult>> getNormalActionResult)
        {
            if (status.HasErrors)
                return Task.FromResult(status.ToActionResult());

            return getNormalActionResult();
        }
    }
}