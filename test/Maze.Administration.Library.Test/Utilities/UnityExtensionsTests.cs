using System;
using System.Reflection;
using Maze.Administration.Library.Unity;
using Unity;
using Unity.Exceptions;
using Unity.Lifetime;
using Unity.RegistrationByConvention;
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

        [Fact]
        public void TestRegisterAssemblyTypesAsImplementedInterfaces()
        {
            var container = new UnityContainer();
            container.RegisterAssemblyTypesAsImplementedInterfaces<ITestInterface1>(Assembly.GetExecutingAssembly(), WithLifetime.Transient);

            var result = container.Resolve<ITestInterface1>();
            Assert.NotNull(result);

            var result2 = container.Resolve<ITestInterface2>();
            Assert.NotNull(result2);
        }

        [Fact]
        public void TestRegisterAssemblyTypes()
        {
            var container = new UnityContainer();
            container.RegisterAssemblyTypes<ITestInterface1>(Assembly.GetExecutingAssembly(), WithLifetime.Transient);

            var result = container.Resolve<ITestInterface1>();
            Assert.NotNull(result);

            Assert.Throws<ResolutionFailedException>(() => container.Resolve<ITestInterface2>());
        }
    }

    public class TestClass : ITestInterface1, ITestInterface2
    {
    }

    public interface ITestInterface1
    {
    }

    public interface ITestInterface2
    {
    }
}