using Orcus.Server.Connection;

namespace Orcus.Server.Library.Exceptions
{
    public class RestArgumentException : RestException
    {
        public RestArgumentException(RestError error) : base(error)
        {
        }
    }
}