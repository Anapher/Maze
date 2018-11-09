using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tasks.Infrastructure.Client.Utilities;
using Xunit;

namespace Tasks.Infrastructure.Client.Tests.Utilities
{
    public class MessageThrottleServiceTests
    {
        [Fact]
        public void TestComputeDelay()
        {
            var result = MessageThrottleService<string>.ComputeDelay(TimeSpan.FromSeconds(1), TimeSpan.FromMinutes(1), 10);
            var result1 = MessageThrottleService<string>.ComputeDelay(TimeSpan.FromSeconds(1), TimeSpan.FromMinutes(1), 1);
            var result2 = MessageThrottleService<string>.ComputeDelay(TimeSpan.FromSeconds(1), TimeSpan.FromMinutes(1), 5);
            var result3 = MessageThrottleService<string>.ComputeDelay(TimeSpan.FromSeconds(1), TimeSpan.FromMinutes(1), 40);
            var result4 = MessageThrottleService<string>.ComputeDelay(TimeSpan.FromSeconds(1), TimeSpan.FromMinutes(1), 60);
            Console.WriteLine(result);
        }
    }
}
