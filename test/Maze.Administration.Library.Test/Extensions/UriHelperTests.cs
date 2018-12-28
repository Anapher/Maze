using System;
using Maze.Administration.Library.Extensions;
using Xunit;

namespace Maze.Administration.Library.Test.Extensions
{
    public class UriHelperTests
    {
        [Theory]
        [InlineData("test", "test2", "test/test2")]
        [InlineData("hello", "world/today", "hello/world/today")]
        [InlineData("hello", "/world/today", "hello/world/today")]
        [InlineData("/hello", "world/today", "/hello/world/today")]
        [InlineData("/hello", "world/today?test=hey", "/hello/world/today?test=hey")]
        public void TestCombineRelativeUris(string baseUri, string relativeUri, string expectedResult)
        {
            var result = UriHelper.CombineRelativeUris(new Uri(baseUri, UriKind.Relative),
                new Uri(relativeUri, UriKind.Relative));

            Assert.Equal(expectedResult, result.ToString());
        }
    }
}