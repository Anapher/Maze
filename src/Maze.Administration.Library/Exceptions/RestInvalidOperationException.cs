using Orcus.Server.Connection;

namespace Orcus.Administration.Library.Exceptions
{
    public class RestInvalidOperationException : RestException
    {
        public RestInvalidOperationException(RestError error) : base(error)
        {
        }
    }
}