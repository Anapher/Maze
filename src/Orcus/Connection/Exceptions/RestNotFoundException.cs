using Orcus.Server.Connection;

namespace Orcus.Connection.Exceptions
{
    public class RestNotFoundException : RestException
    {
        public RestNotFoundException(RestError error) : base(error)
        {
        }
    }
}