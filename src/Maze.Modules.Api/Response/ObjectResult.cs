using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Maze.Modules.Api.Extensions;
using Maze.Modules.Api.Formatters;
using Maze.Modules.Api.Services;

namespace Maze.Modules.Api.Response
{
    public class ObjectResult : IActionResult
    {
        public ObjectResult(object value)
        {
            Value = value;
            ContentTypes = new MediaTypeCollection();
        }

        public object Value { get; set; }

        /// <summary>
        ///     Gets or sets the HTTP status code.
        /// </summary>
        public int? StatusCode { get; set; }

        public MediaTypeCollection ContentTypes { get; set; }
        public Type DeclaredType { get; set; }
        public IList<IOutputFormatter> Formatters { get; set; }

        public Task ExecuteResultAsync(ActionContext context)
        {
            if (StatusCode.HasValue)
                context.Context.Response.StatusCode = StatusCode.Value;

            return context.Context.RequestServices.GetRequiredService<IActionResultExecutor<ObjectResult>>()
                .ExecuteAsync(context, this);
        }
    }
}