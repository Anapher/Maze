using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Orcus.Modules.Api.Response;

namespace Orcus.Sockets.Tests.OrcusServerTests
{
    public class TestEmptyResponse : OrcusServerTestBase
    {
        protected override Task WriteResponse(OrcusResponse response)
        {
            response.StatusCode = StatusCodes.Status404NotFound;
            return Task.CompletedTask;
        }
    }
}