using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Maze.Server.Connection.Utilities;
using Xunit;

namespace Maze.Server.Connection.Tests.Utilities
{
    public class NameGeneratorUtilitiesTests
    {
        [Theory]
        [InlineData("test test.pdf", "test test.pdf")]
        [InlineData("test/ test.pdf", "test_ test.pdf")]
        [InlineData("test? test.pdf", "test_ test.pdf")]
        [InlineData("<wtf>.mp4", "_wtf_.mp4")]
        public void TestToFilenameWithSpaces(string source, string expectedResult)
        {
            var result = NameGeneratorUtilities.ToFilename(source, includeSpace: true);
            Assert.Equal(expectedResult, result);
        }

        [Theory]
        [InlineData("test test.pdf", "testTest.pdf")]
        [InlineData("test/ test.pdf", "test_Test.pdf")]
        [InlineData("test? test.pdf", "test_Test.pdf")]
        [InlineData("<wtf>.mp4", "_wtf_.mp4")]
        [InlineData("test.mp4", "test.mp4")]
        [InlineData("hello world.mp4", "helloWorld.mp4")]
        public void TestToFilenameWithoutSpaces(string source, string expectedResult)
        {
            var result = NameGeneratorUtilities.ToFilename(source, includeSpace: false);
            Assert.Equal(expectedResult, result);
        }

        [Theory]
        [InlineData("test.mp4", "", "test.mp4")]
        [InlineData("test.mp4", "test.mp4", "test_1.mp4")]
        [InlineData("test", "", "test")]
        [InlineData("test", "test", "test_1")]
        [InlineData("test.pdf", "test.pdf|test_1.pdf|test_2.pdf", "test_3.pdf")]
        public void TestMakeUnique(string filename, string existing, string expectedResult)
        {
            var existingNames = existing.Split('|');

            var result = NameGeneratorUtilities.MakeUnique(filename, "_[N]", s => existingNames.Contains(s));
            Assert.Equal(expectedResult, result);
        }
    }
}
