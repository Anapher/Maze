namespace Tasks.Infrastructure.Core
{
    /// <summary>
    ///     Important key words for XML serializing <see cref="OrcusTask"/>
    /// </summary>
    public static class XmlNames
    {
        public const string Root = "task";
        public const string Metadata = "metadata";
        public const string Name = "name";
        public const string Id = "id";
        public const string Audience = "audience";
        public const string Filters = "filters";
        public const string Triggers = "triggers";
        public const string Stop = "stop";
        public const string Commands = "commands";
        public const string Command = "Command";
        public const string CommandName = "name";
        public const string CommandModules = "modules";
    }
}