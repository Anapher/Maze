namespace Orcus.Modules.Api.Response
{
    /// <summary>
    ///     An <see cref="T:Microsoft.AspNetCore.Mvc.StatusCodeResult" /> that when executed will produce an empty
    ///     <see cref="F:Microsoft.AspNetCore.Http.StatusCodes.Status200OK" /> response.
    /// </summary>
    public class OkResult : StatusCodeResult
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:Microsoft.AspNetCore.Mvc.OkResult" /> class.
        /// </summary>
        public OkResult() : base(200)
        {
        }
    }
}