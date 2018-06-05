namespace Orcus.Server.OrcusSockets.Internal
{
    internal static class Headers
    {
        public const string Upgrade = "Upgrade";
        public const string SecWebSocketKey = "Sec-WebSocket-Key";
        public const string SecWebSocketVersion = "Sec-WebSocket-Version";
        public const string Connection = "Connection";

        public const string UpgradeSocket = "orcussocket";
        public const string ConnectionUpgrade = "Upgrade";
        public const string SecWebSocketAccept = "Sec-WebSocket-Accept";
        public const string SupportedVersion = "1";
    }
}