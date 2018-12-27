using Orcus.Server.Connection;

namespace Orcus.Server.Library.Exceptions
{
    public class RestAuthenticationException : RestException
    {
        public RestAuthenticationException(RestError error) : base(error)
        {
        }
    }
}