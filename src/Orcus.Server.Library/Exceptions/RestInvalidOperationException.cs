using Orcus.Server.Connection;

namespace Orcus.Server.Library.Exceptions
{
    public class RestInvalidOperationException : RestException
    {
        public RestInvalidOperationException(RestError error) : base(error)
        {
        }
    }
}