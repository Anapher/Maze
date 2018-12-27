using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using Serilog.Extensions.Logging;
using Xunit.Abstractions;

namespace Orcus.Server.BusinessLogic.Tests.Authentication
{
    public abstract class BusinessLogicTestBase<TBusinessLogic>
    {
        protected ILogger<TBusinessLogic> Logger;

        protected BusinessLogicTestBase(ITestOutputHelper output)
        {
            var logger = new LoggerConfiguration().WriteTo.TestOutput(output).MinimumLevel.Is(LogEventLevel.Verbose).CreateLogger();
            var provider = new SerilogLoggerProvider(logger);
            var factory = new LoggerFactory(new[] {provider});
            Logger = factory.CreateLogger<TBusinessLogic>();
        }
    }
}