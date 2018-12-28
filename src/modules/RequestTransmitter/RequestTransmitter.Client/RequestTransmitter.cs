using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Maze.Client.Library.Services;
using RequestTransmitter.Client.Options;
using RequestTransmitter.Client.Storage;
using RequestTransmitter.Client.Utilities;

namespace RequestTransmitter.Client
{
    public class RequestTransmitter : IRequestTransmitter
    {
        private readonly ICoreConnector _coreConnector;
        private readonly ILogger<RequestTransmitter> _logger;
        private readonly RequestTransmitterOptions _options;
        private readonly IRequestStorage _requestStorage;
        private readonly IServiceProvider _serviceProvider;

        public RequestTransmitter(ICoreConnector coreConnector, IServiceProvider serviceProvider, IRequestStorage requestStorage,
            IOptions<RequestTransmitterOptions> options, ILogger<RequestTransmitter> logger)
        {
            _coreConnector = coreConnector;
            _serviceProvider = serviceProvider;
            _requestStorage = requestStorage;
            _options = options.Value;
            _logger = logger;
        }

        public Task<bool> Transmit(HttpRequestMessage requestMessage) => Transmit<NullResponseCallback>(requestMessage);

        public async Task<bool> Transmit<TResponseCallback>(HttpRequestMessage requestMessage) where TResponseCallback : IResponseCallback
        {
            var connection = _coreConnector.CurrentConnection;
            if (connection != null)
                using (var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(_options.RequestTimeoutSeconds)))
                {
                    try
                    {
                        var response = await connection.RestClient.SendMessage(requestMessage, cancellationTokenSource.Token);
                        if (typeof(TResponseCallback) == typeof(NullResponseCallback))
                        {
                            response.Dispose();
                            return true;
                        }

                        using (var scope = _serviceProvider.CreateScope())
                        {
                            var responseCallback = scope.ServiceProvider.GetRequiredService<TResponseCallback>();
                            try
                            {
                                await responseCallback.ResponseReceived(response);
                            }
                            catch (Exception e)
                            {
                                _logger.LogWarning(e, "An error occurred when invoking response callback {responseCallback}",
                                    typeof(TResponseCallback).FullName);
                            }
                        }

                        return true;
                    }
                    catch (OperationCanceledException)
                    {
                        _logger.LogInformation("Request {requestId} timed out.", requestMessage.RequestUri);
                    }
                    catch (Exception e)
                    {
                        _logger.LogWarning(e, "Request {requestId} failed.", requestMessage.RequestUri);
                    }
                }

            if (typeof(TResponseCallback) != typeof(NullResponseCallback))
                requestMessage.Headers.Add("Response-Callback", typeof(IResponseCallback).AssemblyQualifiedName);

            await _requestStorage.Push(requestMessage);
            return false;
        }
    }
}