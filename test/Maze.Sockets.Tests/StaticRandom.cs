using System;
using System.Threading;

namespace Maze.Sockets.Tests
{
    //https://stackoverflow.com/a/19271062
    public static class StaticRandom
    {
        static int _seed = Environment.TickCount;

        static readonly ThreadLocal<Random> Random =
            new ThreadLocal<Random>(() => new Random(Interlocked.Increment(ref _seed)));

        public static int Next()
        {
            return Random.Value.Next();
        }

        public static int Next(int maxValue)
        {
            return Random.Value.Next(maxValue);
        }

        public static int Next(int minValue, int maxValue)
        {
            return Random.Value.Next(minValue, maxValue);
        }

        public static void NextBytes(byte[] buffer)
        {
            Random.Value.NextBytes(buffer);
        }
    }
}