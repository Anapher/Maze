using System;

namespace Orcus.Modules.Api.Response
{
    public class ExceptionResult : ObjectResult
    {
        public ExceptionResult(Exception exception) : base(OrcusError.FromException(exception))
        {
        }
    }
}