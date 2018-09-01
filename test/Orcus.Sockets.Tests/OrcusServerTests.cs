using System.IO;
using System.Net.Http;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Orcus.Sockets.Tests
{
    public class OrcusServerTests
    {
        [Fact]
        public async Task TestSendRequest()
        {
            var stream = new MemoryStream();
            var socket = new OrcusSocket(stream, null);

            var server = new OrcusServer(socket, 1024, 8192);

            var requestMessage =
                new HttpRequestMessage(HttpMethod.Get, "http://localhost/test")
                {
                    Content = new StringContent("Hello World!")
                };
            var requestTask = server.SendRequest(requestMessage, CancellationToken.None); //will wait for a response
            await Task.Delay(20);

            stream.Seek(0, SeekOrigin.Begin);

            var receiverSocket = new OrcusSocket(stream, null);
            var receiverServer = new OrcusServer(receiverSocket, 1024, 8192);

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

            Assert.Equal(requestMessage.Method.Method, result.Request.Method);
            Assert.Equal(requestMessage.RequestUri.LocalPath, result.Request.Path);
            Assert.Equal("Hello World!", new StreamReader(result.Request.Body).ReadToEnd());

            stream.SetLength(0);

            result.Response.StatusCode = 303;

            var testResponse = Encoding.UTF8.GetBytes("Hello Universe!");
            result.Response.Body.Write(testResponse, 0, testResponse.Length);

            await receiverServer.FinishResponse(result);

            stream.Seek(0, SeekOrigin.Begin);
            try
            {
                await socket.ReceiveAsync();
            }
            catch (WebSocketException) //EOF
            {
            }

            var response = await requestTask;

            Assert.Equal(303, (int) response.StatusCode);
            Assert.Equal("Hello Universe!", await response.Content.ReadAsStringAsync());
        }
    }
}