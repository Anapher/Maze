using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NuGetLogger = NuGet.Common.ILogger;
using ILogMessage = NuGet.Common.ILogMessage;
using LogLevel = NuGet.Common.LogLevel;

namespace Orcus.Server.Service
{
    public class NuGetLoggerWrapper : NuGetLogger
    {
        private readonly ILogger _logger;

        public NuGetLoggerWrapper(ILogger logger)
        {
            _logger = logger;
        }

        public void LogDebug(string data)
        {
            _logger.LogDebug(data);
        }

        public void LogVerbose(string data)
        {
            _logger.LogTrace(data);
        }

        public void LogInformation(string data)
        {
            _logger.LogInformation(data);
        }

        public void LogMinimal(string data)
        {
            _logger.LogInformation(data);
        }

        public void LogWarning(string data)
        {
            _logger.LogWarning(data);
        }

        public void LogError(string data)
        {
            _logger.LogError(data);
        }

        public void LogInformationSummary(string data)
        {
            _logger.LogInformation(data);
        }

        public void Log(LogLevel level, string data)
        {
            switch (level)
            {
                case LogLevel.Debug:
                    LogDebug(data);
                    break;
                case LogLevel.Information:
                    LogInformation(data);
                    break;
                case LogLevel.Warning:
                    LogWarning(data);
                    break;
                case LogLevel.Error:
                    LogError(data);
                    break;
                case LogLevel.Verbose:
                    LogVerbose(data);
                    break;
                case LogLevel.Minimal:
                    LogMinimal(data);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(level), level, null);
            }
        }

        public Task LogAsync(LogLevel level, string data)
        {
            Log(level, data);
            return Task.CompletedTask;
        }

        public void Log(ILogMessage message)
        {
            Log(message.Level, message.Message);
        }

        public Task LogAsync(ILogMessage message)
        {
            Log(message);
            return Task.CompletedTask;
        }
    }
}
