using Orcus.Server.Connection;

namespace Orcus.Server.Library.Exceptions
{
    public class RestNotFoundException : RestException
    {
        public RestNotFoundException(RestError error) : base(error)
        {
        }
    }
}