using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Orcus.Client.Library.Interfaces;
using Orcus.Core.Modules;
using Orcus.Logging;
using Orcus.ModuleManagement;
using Orcus.Options;
using Orcus.Service.Commander;

namespace Orcus.Core.Connection
{
    public interface ICoreConnector
    {
        ServerConnection CurrentConnection { get; }
        Task StartConnecting(ILifetimeScope lifetimeScope);
    }

    public class CoreConnector : ICoreConnector
    {
        private static readonly ILog Logger = LogProvider.For<CoreConnector>();

        private readonly IServerConnector _serverConnector;
        private readonly IModuleDownloader _moduleDownloader;
        private readonly IPackageLockLoader _packageLockLoader;
        private readonly ConnectionOptions _options;

        public CoreConnector(IOptions<ConnectionOptions> options, IServerConnector serverConnector,
            IModuleDownloader moduleDownloader, IPackageLockLoader packageLockLoader)
        {
            _serverConnector = serverConnector;
            _moduleDownloader = moduleDownloader;
            _packageLockLoader = packageLockLoader;
            _options = options.Value;
        }

        public ServerConnection CurrentConnection { get; private set; }
        public ILifetimeScope CurrentConnectionScope { get; private set; }

        public async Task StartConnecting(ILifetimeScope lifetimeScope)
        {
            var uris = new List<Uri>();
            foreach (var serverUri in _options.ServerUris)
            {
                if (Uri.TryCreate(serverUri, UriKind.Absolute, out var uri))
                    uris.Add(uri);
                else Logger.Warn("Unable to parse uri {uri}", serverUri);
            }

            while (true)
            {
                foreach (var serverUri in uris)
                {
                    Logger.Debug("Try to connect to {uri}", serverUri);
                    try
                    {
                        var connection = await _serverConnector.TryConnect(serverUri);
                        Logger.Debug("Connection succeeded, initialize modules");

                        await _moduleDownloader.Load(connection.PackagesLock, connection, CancellationToken.None);

                        var loadedContext = await _packageLockLoader.Load(connection.PackagesLock);
                        var controllers = await loadedContext.GetControllers();

                        CurrentConnectionScope = lifetimeScope.BeginLifetimeScope(builder =>
                        {
                            if (loadedContext.PackagesLoaded)
                                loadedContext.Configure(builder);
                            
                            builder.RegisterOrcusServices(cache => cache.BuildCache(controllers));
                            builder.RegisterType<AutofacServiceProvider>().AsImplementedInterfaces();
                        });

                        CurrentConnection = connection;

                        await CurrentConnectionScope.Execute<IConnectedAction>();
                        await CurrentConnection.InitializeWebSocket(CurrentConnectionScope);

                        break;
                    }
                    catch (Exception e)
                    {
                        Logger.Warn(e, "Error occurred when trying to connect to {uri}", serverUri);
                    }
                }

                if (CurrentConnection != null)
                    break;

                await Task.Delay(_options.ReconnectDelay);
            }
        }
    }
}