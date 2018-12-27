using Orcus.Server.Connection;

namespace Orcus.Exceptions
{
    public class RestNotFoundException : RestException
    {
        public RestNotFoundException(RestError error) : base(error)
        {
        }
    }
}