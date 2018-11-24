using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Extensions.Logging;
using Serilog.Formatting;
using Serilog.Formatting.Compact;
using ILogger = Microsoft.Extensions.Logging.ILogger;

#if NETCOREAPP2_1
namespace Tasks.Infrastructure.Server.Library

#else
namespace Tasks.Infrastructure.Client.Library

#endif
{
    public abstract class LoggingTaskExecutor : ILogger
    {
        private readonly MemoryStringSink _sink;

        protected LoggingTaskExecutor()
        {
            _sink = new MemoryStringSink(new CompactJsonFormatter());

            Logger =
                new SerilogLoggerProvider(new LoggerConfiguration().MinimumLevel.Debug().WriteTo
                    .Sink(_sink).CreateLogger()).CreateLogger("LoggingTaskExecutor");
        }

        public ILogger Logger { get; }

        public IDisposable BeginScope<TState>(TState state) => Logger.BeginScope(state);

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            Logger.Log(logLevel, eventId, state, exception, formatter);
        }

        protected HttpResponseMessage Log(HttpStatusCode statusCode) =>
            new HttpResponseMessage(statusCode)
            {
                Content = new StringContent(_sink.ToString()) {Headers = {ContentType = new MediaTypeHeaderValue("orcus/jsonlog")}}
            };

        protected HttpResponseMessage NotExecuted(string reason) =>
            new HttpResponseMessage(HttpStatusCode.BadRequest) {Content = new StringContent($"The command was not executed.\r\nReason: {reason}")};
    }

    public class MemoryStringSink : ILogEventSink
    {
        private readonly ITextFormatter _formatter;
        private readonly StringBuilder _stringBuilder;
        private readonly StringWriter _stringWriter;
        private readonly object _syncLock = new object();

        public MemoryStringSink(ITextFormatter formatter)
        {
            _formatter = formatter;
            _stringBuilder = new StringBuilder();
            _stringWriter = new StringWriter(_stringBuilder);
        }

        public void Emit(LogEvent logEvent)
        {
            lock (_syncLock)
            {
                _formatter.Format(logEvent, _stringWriter);
            }
        }

        public override string ToString() => _stringBuilder.ToString();
    }
}