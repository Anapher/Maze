namespace Orcus.Server.Connection.Commanding
{
    public struct CommandTarget
    {
        public CommandTarget(CommandTargetType type, int from, int to)
        {
            Type = type;
            To = to;
            From = from;
        }

        public CommandTarget(CommandTargetType type, int id)
        {
            Type = type;
            From = id;
            To = id;
        }

        public int From { get; }
        public int To { get; }
        public CommandTargetType Type { get; set; }
    }
}
