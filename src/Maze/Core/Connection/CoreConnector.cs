using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Microsoft.Extensions.Options;
using Maze.Client.Library.Interfaces;
using Maze.Client.Library.Services;
using Maze.Core.Configuration;
using Maze.Core.Modules;
using Maze.Core.Rest.ClientConfiguration.V1;
using Maze.Logging;
using Maze.ModuleManagement;
using Maze.Options;
using Maze.Service.Commander;

namespace Maze.Core.Connection
{
    public interface IManagementCoreConnector : ICoreConnector
    {
        Task StartConnecting(ILifetimeScope lifetimeScope);
    }

    public class CoreConnector : IManagementCoreConnector
    {
        private static readonly ILog Logger = LogProvider.For<CoreConnector>();

        private readonly IServerConnector _serverConnector;
        private readonly IModuleDownloader _moduleDownloader;
        private readonly IPackageLockLoader _packageLockLoader;
        private readonly IConfigurationManager _configurationManager;
        private readonly IPackageLockUpdater _packageLockUpdater;
        private readonly IMazeProcessController _processController;
        private readonly ConnectionOptions _options;

        public CoreConnector(IOptions<ConnectionOptions> options, IServerConnector serverConnector, IModuleDownloader moduleDownloader,
            IPackageLockLoader packageLockLoader, IConfigurationManager configurationManager, IPackageLockUpdater packageLockUpdater, IMazeProcessController processController)
        {
            _serverConnector = serverConnector;
            _moduleDownloader = moduleDownloader;
            _packageLockLoader = packageLockLoader;
            _configurationManager = configurationManager;
            _packageLockUpdater = packageLockUpdater;
            _processController = processController;
            _options = options.Value;
        }

        public IServerConnection CurrentConnection { get; private set; }
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

                        Logger.Debug("Connection succeeded, download modules");
                        await _moduleDownloader.Load(connection.PackagesLock, connection, CancellationToken.None);

                        Logger.Debug("Modules downloaded successfully, loading configuration");
                        var configurationInfo = await ClientConfigurationResource.GetConfigurationsInfo(connection.RestClient);
                        if (!_configurationManager.IsSynchronized(configurationInfo))
                        {
                            var configurations = await ClientConfigurationResource.GetConfigurations(connection.RestClient);
                            _configurationManager.Synchronize(configurations, connection.PackagesLock);
                        }

                        if (!_packageLockUpdater.Verify(connection.PackagesLock))
                        {
                            _processController.Restart();
                            return;
                        }

                        var loadedContext = await _packageLockLoader.Load(connection.PackagesLock);
                        var controllers = await loadedContext.GetControllers();

                        var restClientWrapper = lifetimeScope.Resolve<MazeRestClientWrapper>();
                        restClientWrapper.Initialize(connection.RestClient);

                        CurrentConnectionScope = lifetimeScope.BeginLifetimeScope(builder =>
                        {
                            if (loadedContext.PackagesLoaded)
                                loadedContext.Configure(builder);

                            builder.RegisterMazeServices(cache => cache.BuildCache(controllers));
                        });

                        CurrentConnection = connection;

                        using (CurrentConnectionScope)
                        {
                            await CurrentConnectionScope.Execute<IConnectedAction>();
                            await connection.InitializeWebSocket(CurrentConnectionScope);
                        }

                        CurrentConnection = null;
                        restClientWrapper.Initialize(null);
                        break;
                    }
                    catch (Exception e)
                    {
                        Logger.Warn(e, "Error occurred when trying to connect to {uri}", serverUri);
                    }
                }
                
                await Task.Delay(_options.ReconnectDelay);
            }
        }
    }
}