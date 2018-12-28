using System;

namespace Maze.Modules.Api.Response
{
    public class StatusCodeResult : ActionResult
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:Microsoft.AspNetCore.Mvc.StatusCodeResult" /> class
        ///     with the given <paramref name="statusCode" />.
        /// </summary>
        /// <param name="statusCode">The HTTP status code of the response.</param>
        public StatusCodeResult(int statusCode)
        {
            StatusCode = statusCode;
        }

        /// <summary>Gets the Maze status code.</summary>
        public int StatusCode { get; }

        /// <inheritdoc />
        public override void ExecuteResult(ActionContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            context.Context.Response.StatusCode = StatusCode;
        }
    }
}