using System.Collections.Generic;

namespace Tasks.Infrastructure.Core.Commands
{
    public class CommandMetadata
    {
        public string Name { get; set; }
        public IList<string> Modules { get; set; }
    }
}