using Tasks.Infrastructure.Administration.PropertyGrid;
using Xunit;

namespace Tasks.Infrastructure.Administration.Tests.PropertyGrid
{
    public class PropertyTests
    {
        private class TestClass
        {
            public string Test { get; set; }
        }

        private class TestClass2
        {
            public TestClass TestClass { get; } = new TestClass();
        }

        [Fact]
        public void TestNestedProperty()
        {
            var test = new TestClass2();
            var property = new Property<string>(null, () => test.TestClass.Test, null, null, null);
            property.Value = "Hello World";

            Assert.Equal("Hello World", test.TestClass.Test);
        }

        [Fact]
        public void TestSimpleProperty()
        {
            var test = new TestClass();
            var property = new Property<string>(null, () => test.Test, null, null, null);
            property.Value = "Hello World";

            Assert.Equal("Hello World", test.Test);
        }
    }
}