using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Orcus.Modules.Api.Request;
using Orcus.Modules.Api.Response;
using Xunit;

namespace Orcus.Sockets.Tests.OrcusServerTests
{
    public class TestSendShortMessageReceiveResponse : OrcusServerTestBase
    {
        protected override HttpRequestMessage GetRequest()
        {
            return new HttpRequestMessage(HttpMethod.Get, "http://localhost/test")
            {
                Content = new StringContent("Hello World!")
            };
        }

        protected override async Task WriteResponse(OrcusResponse response)
        {
            response.StatusCode = 303;

            var testResponse = Encoding.UTF8.GetBytes("Hello Universe!");
            await response.Body.WriteAsync(testResponse, 0, testResponse.Length);
        }

        protected override async Task AssertReceivedRequest(HttpRequestMessage requestMessage, OrcusRequest request)
        {
            await base.AssertReceivedRequest(requestMessage, request);
            Assert.Equal("Hello World!", await new StreamReader(request.Body).ReadToEndAsync());
        }

        protected override async Task AssertReceivedResponse(OrcusResponse response, HttpResponseMessage responseMessage)
        {
            await base.AssertReceivedResponse(response, responseMessage);

            Assert.Equal("Hello Universe!", await responseMessage.Content.ReadAsStringAsync());
        }
    }
}