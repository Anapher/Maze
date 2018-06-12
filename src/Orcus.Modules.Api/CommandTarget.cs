using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Orcus.Modules.Api
{
    public class CommandTargetCollection : Collection<CommandTarget>
    {
        public bool TargetsServer => this.Any(x => x.Type == CommandTargetType.Server);
    }

    public class CommandTarget
    {
        public CommandTarget(CommandTargetType type)
        {
            Type = type;
        }

        public CommandTargetType Type { get; }
        public int Id { get; set; }

        public static CommandTarget Server { get; } = new CommandTarget(CommandTargetType.Server);
    }

    public enum CommandTargetType
    {
        Server,
        Client
    }
}
