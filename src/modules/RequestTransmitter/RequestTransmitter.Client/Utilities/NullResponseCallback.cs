using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace RequestTransmitter.Client.Utilities
{
    internal class NullResponseCallback : IResponseCallback
    {
        public Task ResponseReceived(HttpResponseMessage responseMessage)
        {
            throw new NotSupportedException();
        }
    }
}