using System;
using Maze.Sockets.Internal;
using Xunit;

namespace Maze.Sockets.Tests.Internal
{
    public class BinaryUtilsTest
    {
        [Fact]
        public void TestWriteInt32()
        {
            var buffer = new byte[20];
            var c = BinaryUtils.WriteInt32(buffer, 2, 1289);

            Assert.Equal(4, c);
            Assert.Equal(1289, BitConverter.ToInt32(buffer, 2));
        }
    }
}