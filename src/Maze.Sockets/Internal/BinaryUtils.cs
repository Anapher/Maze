using System.Runtime.CompilerServices;

namespace Maze.Sockets.Internal
{
    /// <summary>
    ///     Binary utilities for writing data to byte arrays
    /// </summary>
    public static class BinaryUtils
    {
        /// <summary>
        ///     Write the <see cref="int" /> value to a byte array
        /// </summary>
        /// <param name="bytes">The byte array</param>
        /// <param name="offset">The offset in the <see cref="bytes" /> where the value should be written</param>
        /// <param name="value">The <see cref="int" /> that should be encoded</param>
        /// <returns>Return the amount of bytes written to the <see cref="bytes" /></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe int WriteInt32(byte[] bytes, int offset, int value)
        {
            fixed (byte* ptr = bytes)
            {
                *(int*) (ptr + offset) = value;
            }

            return 4;
        }
    }
}