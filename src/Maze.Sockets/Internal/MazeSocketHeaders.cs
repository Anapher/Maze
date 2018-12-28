namespace Maze.Sockets.Internal
{
    public static class MazeSocketHeaders
    {
        public const string Upgrade = "Upgrade";
        public const string SecWebSocketKey = "Sec-WebSocket-Key";
        public const string SecWebSocketVersion = "Sec-WebSocket-Version";
        public const string Connection = "Connection";

        public const string UpgradeSocket = "websocket";
        public const string ConnectionUpgrade = "Upgrade";
        public const string SecWebSocketAccept = "Sec-WebSocket-Accept";
        public const string SupportedVersion = "13";
    }
}