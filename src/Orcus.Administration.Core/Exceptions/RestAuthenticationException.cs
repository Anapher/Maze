using Orcus.Server.Connection;

namespace Orcus.Administration.Core.Exceptions
{
    public class RestAuthenticationException : RestException
    {
        public RestAuthenticationException(RestError error) : base(error)
        {
        }
    }
}