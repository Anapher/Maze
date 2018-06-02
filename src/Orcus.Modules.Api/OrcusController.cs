using System;
using System.IO;
using Orcus.Modules.Api.Response;

namespace Orcus.Modules.Api
{
    public abstract class OrcusController : IDisposable
    {
        public OrcusContext OrcusContext { get; set; }

        [NonAction]
        public virtual void Dispose()
        {
        }

        [NonAction]
        public virtual OkObjectResult Ok(object value)
        {
            return new OkObjectResult(value);
        }

        [NonAction]
        public virtual OkResult Ok()
        {
            return new OkResult();
        }

        /// <summary>
        ///     Creates a <see cref="StatusCodeResult" /> object by specifying a
        ///     <paramref name="statusCode" />.
        /// </summary>
        /// <param name="statusCode">The status code to set on the response.</param>
        /// <returns>The created <see cref="StatusCodeResult" /> object for the response.</returns>
        [NonAction]
        public virtual StatusCodeResult StatusCode(int statusCode)
        {
            return new StatusCodeResult(statusCode);
        }

        /// <summary>
        ///     Creates a <see cref="ObjectResult" /> object by specifying a
        ///     <paramref name="statusCode" /> and <paramref name="value" />
        /// </summary>
        /// <param name="statusCode">The status code to set on the response.</param>
        /// <param name="value">The value to set on the <see cref="ObjectResult" />.</param>
        /// <returns>The created <see cref="ObjectResult" /> object for the response.</returns>
        [NonAction]
        public virtual ObjectResult StatusCode(int statusCode, object value)
        {
            return new ObjectResult(value)
            {
                StatusCode = statusCode
            };
        }

        /// <summary>
        ///     Returns a file in the specified <paramref name="fileStream" /> (
        ///     <see cref="F:Microsoft.AspNetCore.Http.StatusCodes.Status200OK" />), with the
        ///     specified <paramref name="contentType" /> as the Content-Type.
        ///     This supports range requests (<see cref="F:Microsoft.AspNetCore.Http.StatusCodes.Status206PartialContent" /> or
        ///     <see cref="F:Microsoft.AspNetCore.Http.StatusCodes.Status416RangeNotSatisfiable" /> if the range is not
        ///     satisfiable).
        /// </summary>
        /// <param name="fileStream">The <see cref="T:System.IO.Stream" /> with the contents of the file.</param>
        /// <param name="contentType">The Content-Type of the file.</param>
        /// <returns>The created <see cref="T:Microsoft.AspNetCore.Mvc.FileStreamResult" /> for the response.</returns>
        [NonAction]
        public virtual FileStreamResult File(Stream fileStream, string contentType)
        {
            return File(fileStream, contentType, null);
        }

        /// <summary>
        ///     Returns a file in the specified <paramref name="fileStream" /> (
        ///     <see cref="F:Microsoft.AspNetCore.Http.StatusCodes.Status200OK" />) with the
        ///     specified <paramref name="contentType" /> as the Content-Type and the
        ///     specified <paramref name="fileDownloadName" /> as the suggested file name.
        ///     This supports range requests (<see cref="F:Microsoft.AspNetCore.Http.StatusCodes.Status206PartialContent" /> or
        ///     <see cref="F:Microsoft.AspNetCore.Http.StatusCodes.Status416RangeNotSatisfiable" /> if the range is not
        ///     satisfiable).
        /// </summary>
        /// <param name="fileStream">The <see cref="T:System.IO.Stream" /> with the contents of the file.</param>
        /// <param name="contentType">The Content-Type of the file.</param>
        /// <param name="fileDownloadName">The suggested file name.</param>
        /// <returns>The created <see cref="T:Microsoft.AspNetCore.Mvc.FileStreamResult" /> for the response.</returns>
        [NonAction]
        public virtual FileStreamResult File(Stream fileStream, string contentType, string fileDownloadName)
        {
            var fileStreamResult =
                new FileStreamResult(fileStream, contentType) {FileDownloadName = fileDownloadName};
            return fileStreamResult;
        }

        [NonAction]
        public virtual ExceptionResult Exception(Exception exception)
        {
            return new ExceptionResult(exception);
        }
    }
}