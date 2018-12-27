using Microsoft.AspNetCore.Http.Headers;
using Orcus.Modules.Api.Request;
using Orcus.Modules.Api.Response;

namespace Orcus.Modules.Api.Extensions
{
    public static class HeaderDictionaryTypeExtensions
    {
        public static RequestHeaders GetTypedHeaders(this OrcusRequest request)
        {
            return new RequestHeaders(request.Headers);
        }

        public static ResponseHeaders GetTypedHeaders(this OrcusResponse request)
        {
            return new ResponseHeaders(request.Headers);
        }
    }
}