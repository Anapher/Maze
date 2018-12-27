using System.Buffers;
using System.IO;
using System.Net.Http;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Orcus.Modules.Api.Request;
using Orcus.Modules.Api.Response;
using Xunit;

namespace Orcus.Sockets.Tests.OrcusServerTests
{
    public abstract class OrcusServerTestBase
    {
        protected virtual int PackageSize { get; } = 1024;
        protected virtual int MaxHeaderSize { get; } = 1024;

        protected virtual HttpRequestMessage GetRequest() => new HttpRequestMessage(HttpMethod.Get, "http://localhost/test");

        protected virtual Task AssertReceivedRequest(HttpRequestMessage requestMessage, OrcusRequest request)
        {
            Assert.Equal(requestMessage.Method.Method, request.Method);
            Assert.Equal(requestMessage.RequestUri.LocalPath, request.Path);

            return Task.CompletedTask;
        }

        protected virtual Task AssertReceivedResponse(OrcusResponse response, HttpResponseMessage responseMessage)
        {
            Assert.Equal(response.StatusCode, (int) responseMessage.StatusCode);

            return Task.CompletedTask;
        }

        protected abstract Task WriteResponse(OrcusResponse response);

        [Fact]
        public async Task ExecuteTest()
        {
            var dataStream = new MemoryStream();
            var requestSocket = new OrcusSocket(dataStream, keepAliveInterval: null);
            var requestServer = new OrcusServer(requestSocket, PackageSize, MaxHeaderSize, ArrayPool<byte>.Shared);

            var request = GetRequest();
            var requestTask = requestServer.SendRequest(request, CancellationToken.None); //will wait for a response
            await Task.Delay(20);

            dataStream.Position = 0;

            var receiverSocket = new OrcusSocket(dataStream, null);
            var receiverServer = new OrcusServer(receiverSocket, PackageSize, MaxHeaderSize, ArrayPool<byte>.Shared);

            var completionSource = new TaskCompletionSource<OrcusRequestReceivedEventArgs>();

            receiverServer.RequestReceived += (sender, args) => completionSource.SetResult(args);

            try
            {
                await receiverSocket.ReceiveAsync();
            }
            catch (WebSocketException) //EOF
            {
            }

            var result = await completionSource.Task;
            await AssertReceivedRequest(request, result.Request);

            dataStream.SetLength(0);

            await WriteResponse(result.Response);

            await receiverServer.FinishResponse(result);

            dataStream.Seek(0, SeekOrigin.Begin);
            try
            {
                await requestSocket.ReceiveAsync();
            }
            catch (WebSocketException) //EOF
            {
            }

            var response = await requestTask;

            await AssertReceivedResponse(result.Response, response);
        }
    }
}