using System.Collections.Generic;

namespace Tasks.Infrastructure.Core.Commands
{
    /// <summary>
    ///     Metadata of a command
    /// </summary>
    public class CommandMetadata
    {
        /// <summary>
        ///     The name of the command
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     The required modules by the command
        /// </summary>
        public IList<string> Modules { get; set; }
    }
}