using System.Collections.Generic;

namespace Orcus.Server.Connection.Tasks.Commands
{
    public class CommandMetadata
    {
        public string Name { get; set; }
        public IList<string> Modules { get; set; }
    }
}