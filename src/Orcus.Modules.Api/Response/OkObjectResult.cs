namespace Orcus.Modules.Api.Response
{
    /// <summary>
    /// An <see cref="T:Microsoft.AspNetCore.Mvc.ObjectResult" /> that when executed performs content negotiation, formats the entity body, and
    /// will produce a <see cref="F:Microsoft.AspNetCore.Http.StatusCodes.Status200OK" /> response if negotiation and formatting succeed.
    /// </summary>
    public class OkObjectResult : ObjectResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Microsoft.AspNetCore.Mvc.OkObjectResult" /> class.
        /// </summary>
        /// <param name="value">The content to format into the entity body.</param>
        public OkObjectResult(object value) : base(value)
        {
            StatusCode = 200;
        }
    }
}