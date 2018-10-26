using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Orcus.Client.Library.Interfaces;
using Orcus.Client.Library.Services;
using RequestTransmitter.Client.Storage;

namespace RequestTransmitter.Client.Hooks
{
    public class OnConnectedAction : IConnectedAction
    {
        private readonly IRequestStorage _requestStorage;
        private readonly ICoreConnector _coreConnector;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<OnConnectedAction> _logger;

        public OnConnectedAction(IRequestStorage requestStorage, ICoreConnector coreConnector, IServiceProvider serviceProvider,
            ILogger<OnConnectedAction> logger)
        {
            _requestStorage = requestStorage;
            _coreConnector = coreConnector;
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public async Task Execute()
        {
            var timeouts = 0;

            HttpRequestMessage requestMessage;
            while ((requestMessage = await _requestStorage.Peek()) != null)
            {
                var connection = _coreConnector.CurrentConnection;
                if (connection == null)
                    return;

                var responseCallbackName = requestMessage.Headers.FirstOrDefault(x => x.Key == "Response-Callback").Key;
                requestMessage.Headers.Remove("Response-Callback");

                try
                {
                    var response = await connection.RestClient.SendMessage(requestMessage, CancellationToken.None);
                    var popTask = _requestStorage.Pop();

                    try
                    {
                        if (responseCallbackName == null)
                        {
                            response.Dispose();
                            continue;
                        }

                        using (var scope = _serviceProvider.CreateScope())
                        {
                            var responseCallbackType = Type.GetType(responseCallbackName);
                            if (responseCallbackType == null)
                            {
                                response.Dispose();
                                _logger.LogWarning("The callback class {className} was not found.", responseCallbackName);
                                return;
                            }

                            var responseCallback = (IResponseCallback) scope.ServiceProvider.GetRequiredService(responseCallbackType);
                            try
                            {
                                await responseCallback.ResponseReceived(response);
                            }
                            catch (Exception e)
                            {
                                _logger.LogWarning(e, "An error occurred when invoking response callback {responseCallback}",
                                    responseCallbackType.FullName);
                            }
                        }

                    }
                    finally
                    {
                        await popTask;
                    }
                }
                catch (TimeoutException)
                {
                    if (_coreConnector.CurrentConnection == null)
                    {
                        _logger.LogDebug("Connection lost");
                        return;
                    }

                    if (timeouts > 3)
                    {
                        _logger.LogDebug("4 timeouts in a row, seems like the connection was lost");
                        return;
                    }

                    timeouts++;
                }
                catch (Exception e)
                {
                    _logger.LogDebug(e, "An error occurred when sending stored request {uri}", requestMessage.RequestUri);
                }
            }
        }
    }
}
