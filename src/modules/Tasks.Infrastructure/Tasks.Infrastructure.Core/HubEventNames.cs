namespace Tasks.Infrastructure.Core
{
    public static class HubEventNames
    {
        private const string RootName = "Tasks.Infrastructure.";

        public const string TaskRemoved = RootName + "TaskRemoved";
        public const string TaskUpdated = RootName + "TaskUpdated";
        public const string TaskCreated = RootName + "TaskCreated";

        public const string TaskExecutionCreated = RootName + "TaskExecutionCreated";
        public const string TaskCommandResultCreated = RootName + "TaskCommandResultCreated";
        public const string TaskCommandProcess = RootName + "TaskCommandProcess";
        public const string TaskSessionCreated = RootName + "TaskSessionCreated";
    }
}