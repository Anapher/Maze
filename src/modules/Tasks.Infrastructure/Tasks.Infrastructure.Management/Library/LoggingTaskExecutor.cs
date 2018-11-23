using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Net.Http;
using System.Text;

namespace Tasks.Infrastructure.Management.Library
{
    public abstract class LoggingTaskExecutor : ILogger
    {
        private readonly StringBuilder _loggerBuilder;

        public LoggingTaskExecutor()
        {
            _loggerBuilder = new StringBuilder();
        }

        protected HttpResponseMessage Log(HttpStatusCode statusCode)
        {
            return new HttpResponseMessage(statusCode) { Content = new StringContent(_loggerBuilder.ToString()) { Headers = { ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("orcus/log") } } };
        }

        protected HttpResponseMessage NotExecuted(string reason)
        {
            return new HttpResponseMessage(HttpStatusCode.BadRequest) { Content = new StringContent($"The command was not executed.\r\nReason: {reason}") };
        }

        public ILogger Logger => this;

        public IDisposable BeginScope<TState>(TState state)
        {
            throw new NotSupportedException();
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            _loggerBuilder.AppendFormat("[{0}]: {1}\r\n", logLevel, formatter.Invoke(state, exception));
        }
    }
}
