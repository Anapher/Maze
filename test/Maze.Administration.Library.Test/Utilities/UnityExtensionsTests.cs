using Maze.Administration.Library.Unity;
using Unity;
using Unity.Lifetime;
using Xunit;

namespace Maze.Administration.Library.Test.Utilities
{
    public class UnityExtensionsTests
    {
        [Fact]
        public void TestAsImplementedInterfaces()
        {
            var container = new UnityContainer();
            container.AsImplementedInterfaces<TestClass, ContainerControlledLifetimeManager>();

            var test1 = container.Resolve<ITestInterface1>();
            var test2 = container.Resolve<ITestInterface2>();

            Assert.Same(test1, test2);

            var cls = container.Resolve<TestClass>();
            Assert.Same(test1, cls);
        }

        private class TestClass : ITestInterface1, ITestInterface2
        {
        }

        public interface ITestInterface1
        {
        }

        public interface ITestInterface2
        {
        }
    }
}