using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Maze.Server.Connection.Commanding
{
    public class CommandTargetCollection : Collection<CommandTarget>
    {
        private const string ServerKeyword = "Server";

        private CommandTargetCollection(bool targetsServer)
        {
            TargetsServer = targetsServer;
        }

        public CommandTargetCollection()
        {
        }

        public CommandTargetCollection(IList<CommandTarget> targets) : base(targets)
        {
        }

        public bool TargetsServer { get; }

        public bool IsSingleClient(out int clientId)
        {
            clientId = -1;

            if (Count != 1)
                return false;

            var target = this[0];
            if (target.Type != CommandTargetType.Client)
                return false;

            if (target.From != target.To)
                return false;

            clientId = target.From;
            return true;
        }

        public static CommandTargetCollection Server { get; } = new CommandTargetCollection(true);

        public void AddClient(int id)
        {
            Add(new CommandTarget(CommandTargetType.Client, id));
        }

        public void AddGroup(int id)
        {
            Add(new CommandTarget(CommandTargetType.Group, id));
        }

        public static CommandTargetCollection Parse(string value)
        {
            if (string.IsNullOrEmpty(value) || value.Equals(ServerKeyword, StringComparison.OrdinalIgnoreCase))
                return Server;

            var result = new CommandTargetCollection();

            CommandTargetType? targetType = null;
            int? from = null;
            int? to = null;
            var state = 0;

            void AddTarget()
            {
                result.Add(new CommandTarget(targetType.Value, from.Value, to ?? from.Value));

                targetType = null;
                state = 0;
                from = null;
                to = null;
            }

            for (int i = 0; i < value.Length; i++)
            {
                var c = value[i];

                switch (state)
                {
                    case 0:
                        switch (c)
                        {
                            case 'G':
                                targetType = CommandTargetType.Group;
                                break;
                            case 'C':
                                targetType = CommandTargetType.Client;
                                break;
                            default:
                                throw new ArgumentException($"The target type {c} is not known.", nameof(value));
                        }

                        state++;
                        continue;
                    case 1:
                        var start = i;

                        while (i + 1 < value.Length && char.IsDigit(value[i + 1]))
                            i++;

                        from = int.Parse(value.Substring(start, i - start + 1));
                        state++;
                        continue;
                    case 2:
                        if (c == ',')
                        {
                            AddTarget();
                        }
                        else if (c == '-')
                        {
                            state++;
                        }
                        else throw new ArgumentException($"Unexpected character at position {i}: {c}");
                        continue;
                    case 3:
                        start = i;

                        while (i + 1 < value.Length && char.IsDigit(value[i + 1]))
                            i++;

                        to = int.Parse(value.Substring(start, i - start + 1));
                        AddTarget();

                        if (++i < value.Length && value[i] != ',')
                            throw new ArgumentException($"Comma was expected at position {i}");
                        continue;
                }
            }

            if (targetType != null)
                if (state <= 1)
                    throw new ArgumentException("String ended unexpected.");
                else
                    AddTarget();

            return result;
        }

        public override string ToString()
        {
            if (TargetsServer)
                return ServerKeyword;

            if (!this.Any())
                return string.Empty; //Server

            var finalData = new List<CommandTarget>();

            foreach (var type in new[] {CommandTargetType.Client, CommandTargetType.Group})
            {
                CommandTarget previousCommandTarget = default;
                bool firstLoop = true;

                foreach (var commandTarget in this.Where(x => x.Type == type).OrderBy(x => x.From))
                {
                    if (!firstLoop)
                    {
                        if (previousCommandTarget.To >= commandTarget.From - 1)
                        {
                            previousCommandTarget =
                                new CommandTarget(type, previousCommandTarget.From, Math.Max(commandTarget.To, previousCommandTarget.To));
                        }
                        else
                        {
                            finalData.Add(previousCommandTarget);
                            previousCommandTarget = commandTarget;
                        }
                    }
                    else previousCommandTarget = commandTarget;

                    firstLoop = false;
                }

                if (!firstLoop)
                    finalData.Add(previousCommandTarget);
            }

            var stringBuilder = new StringBuilder();
            foreach (var item in finalData)
            {
                switch (item.Type)
                {
                    case CommandTargetType.Group:
                        stringBuilder.Append('G');
                        break;
                    case CommandTargetType.Client:
                        stringBuilder.Append('C');
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                stringBuilder.Append(item.From);
                if (item.To != item.From)
                    stringBuilder.Append('-').Append(item.To);
                stringBuilder.Append(',');
            }

            stringBuilder.Length -= 1;
            return stringBuilder.ToString();
        }
    }
}