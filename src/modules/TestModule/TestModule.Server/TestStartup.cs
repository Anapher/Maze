using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Orcus.Server.Library.Interfaces;

namespace TestModule.Server
{
    public class TestStartup : IStartupAction
    {
        private readonly ILogger<TestStartup> _logger;

        public TestStartup(ILogger<TestStartup> logger)
        {
            _logger = logger;
        }

        public Task Execute()
        {
            _logger.LogInformation("Hello World!");
            return Task.CompletedTask;
        }
    }
}