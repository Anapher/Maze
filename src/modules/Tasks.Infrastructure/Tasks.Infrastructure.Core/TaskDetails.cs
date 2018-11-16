namespace Tasks.Infrastructure.Core
{
    /// <summary>
    /// The level of details of the task should be serialized
    /// </summary>
    public enum TaskDetails
    {
        /// <summary>
        ///     Just the commands and the stop events are serialized
        /// </summary>
        Execution,

        /// <summary>
        ///     The audience is skipped as it is not needed by the client
        /// </summary>
        Client,

        /// <summary>
        /// All details of the task are serialized, as required by the server
        /// </summary>
        Server
    }
}