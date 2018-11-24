using System.Net;
using System.Net.Http;

#if NETCOREAPP2_1
namespace Tasks.Infrastructure.Server.Library

#else
namespace Tasks.Infrastructure.Client.Library

#endif
{
    public abstract class TaskExecutor
    {
        protected HttpResponseMessage NotExecuted(string reason)
        {
            return new HttpResponseMessage(HttpStatusCode.BadRequest) { Content = new StringContent($"The command was not executed.\r\nReason: {reason}") };
        }

        protected HttpResponseMessage Ok()
        {
            return new HttpResponseMessage(HttpStatusCode.OK);
        }

        protected HttpResponseMessage Ok(string message)
        {
            return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(message) };
        }

        protected HttpResponseMessage ServerNotSupported()
        {
            return new HttpResponseMessage(HttpStatusCode.BadRequest) { Content = new StringContent("The server is not supported as target for this command.") };
        }

        protected HttpResponseMessage ClientNotConnected()
        {
            return new HttpResponseMessage(HttpStatusCode.Gone) { Content = new StringContent("The client is not connected.") };
        }

        protected HttpResponseMessage ClientsNotSupported()
        {
            return new HttpResponseMessage(HttpStatusCode.BadRequest) { Content = new StringContent("Clients are not supported as target for this command.") };
        }
    }
}
