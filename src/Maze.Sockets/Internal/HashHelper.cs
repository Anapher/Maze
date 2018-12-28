using System;
using System.Security.Cryptography;

namespace Maze.Sockets.Internal
{
    public static class HashHelper
    {
        public static string HashData(byte[] buffer, int offset, int count)
        {
            using (var md5 = MD5.Create())
            {
                return BitConverter.ToString(md5.ComputeHash(buffer, offset, count)).Replace("-", null);
            }
        }
    }
}