namespace Maze.Server.Connection
{
    public static class HubEventNames
    {
        public const string ModuleInstalled = nameof(ModuleInstalled);
        public const string ModuleUninstalled = nameof(ModuleUninstalled);

        public const string ClientConnected = nameof(ClientConnected);
        public const string ClientDisconnected = nameof(ClientDisconnected);

        public const string ClientGroupCreated = nameof(ClientGroupCreated);
        public const string ClientGroupRemoved = nameof(ClientGroupRemoved);
        public const string ClientGroupUpdated = nameof(ClientGroupUpdated);
    }
}
