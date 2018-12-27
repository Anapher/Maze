using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Orcus.Modules.Api.Extensions;
using Orcus.Modules.Api.Formatters;
using Orcus.Modules.Api.Services;

namespace Orcus.Modules.Api.Response
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