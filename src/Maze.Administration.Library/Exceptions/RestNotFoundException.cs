using Orcus.Server.Connection;

namespace Orcus.Administration.Library.Exceptions
{
    public class RestNotFoundException : RestException
    {
        public RestNotFoundException(RestError error) : base(error)
        {
        }
    }
}