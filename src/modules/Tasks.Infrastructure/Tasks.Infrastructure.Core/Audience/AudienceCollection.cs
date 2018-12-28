using System.Collections.Generic;
using Maze.Server.Connection.Commanding;

namespace Tasks.Infrastructure.Core.Audience
{
    /// <summary>
    ///     A collection of command targets that form an audience for commands
    /// </summary>
    public class AudienceCollection : List<CommandTarget>
    {
        /// <summary>
        ///     The audience includes all clients
        /// </summary>
        public bool IsAll { get; set; }

        /// <summary>
        ///     The audience includes the server
        /// </summary>
        public bool IncludesServer { get; set; }
    }
}