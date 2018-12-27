using Orcus.Server.Connection;

namespace Orcus.Administration.Library.Exceptions
{
    public class RestArgumentException : RestException
    {
        public RestArgumentException(RestError error) : base(error)
        {
        }
    }
}