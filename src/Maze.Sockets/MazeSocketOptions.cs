using System;

namespace Maze.Sockets
{
    /// <summary>
    ///     Options for Maze sockets
    /// </summary>
    public class MazeSocketOptions
    {
        public MazeSocketOptions()
        {
            KeepAliveInterval = TimeSpan.FromMinutes(2);
            PackageBufferSize = 8192;
            MaxHeaderSize = 4096;
        }

        /// <summary>
        ///     Gets or sets the frequency at which to send Ping/Pong keep-alive control frames.
        ///     The default is two minutes.
        /// </summary>
        public TimeSpan KeepAliveInterval { get; set; }

        /// <summary>
        ///     Gets or sets the size of the protocol buffer used to send frames.
        ///     The default is 8kb.
        /// </summary>
        public int PackageBufferSize { get; set; }

        /// <summary>
        ///     The maximum size of the http header (for allocation). Default is 4kb
        /// </summary>
        public int MaxHeaderSize { get; set; }
    }
}