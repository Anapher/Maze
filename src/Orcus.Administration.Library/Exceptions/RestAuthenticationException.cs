using Orcus.Server.Connection;

namespace Orcus.Administration.Library.Exceptions
{
    public class RestAuthenticationException : RestException
    {
        public RestAuthenticationException(RestError error) : base(error)
        {
        }
    }
}