using Orcus.Server.Connection;

namespace Orcus.Exceptions
{
    public class RestAuthenticationException : RestException
    {
        public RestAuthenticationException(RestError error) : base(error)
        {
        }
    }
}