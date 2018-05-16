using Orcus.Server.Connection;

namespace Orcus.Administration.Core.Exceptions
{
    public class RestNotFoundException : RestException
    {
        public RestNotFoundException(RestError error) : base(error)
        {
        }
    }
}