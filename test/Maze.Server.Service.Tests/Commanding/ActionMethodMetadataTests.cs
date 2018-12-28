using System.Threading.Tasks;
using Maze.Service.Commander.Commanding;
using Xunit;

namespace Maze.Server.Service.Tests.Commanding
{
    public class ActionMethodMetadataTests
    {
        [Fact]
        public void TestDetectAsyncMethod()
        {
            var methodInfo = GetType().GetMethod(nameof(TestMethod));
            var metadata = new ActionMethodMetadata(null, methodInfo);
            Assert.True(metadata.IsAsync);
        }

        [Fact]
        public void TestDetectSyncMethod()
        {
            var methodInfo = GetType().GetMethod(nameof(TestDetectSyncMethod));
            var metadata = new ActionMethodMetadata(null, methodInfo);
            Assert.False(metadata.IsAsync);
        }

        public Task TestMethod()
        {
            return Task.CompletedTask;
        }
    }
}