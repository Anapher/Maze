namespace Orcus.Server.Connection.Commanding
{
    /// <summary>
    ///     A target represents a single client/group or a range of clients/groups
    /// </summary>
    public struct CommandTarget
    {
        /// <summary>
        ///     Create a new range
        /// </summary>
        /// <param name="type">The type of the range</param>
        /// <param name="from">The inclusive start id</param>
        /// <param name="to">The inclusive end id</param>
        public CommandTarget(CommandTargetType type, int from, int to)
        {
            Type = type;
            To = to;
            From = from;
        }

        /// <summary>
        ///     Create a single target
        /// </summary>
        /// <param name="type">The type of the target</param>
        /// <param name="id">The id of the target</param>
        public CommandTarget(CommandTargetType type, int id)
        {
            Type = type;
            From = id;
            To = id;
        }

        /// <summary>
        ///     The inclusive starting id of the target range
        /// </summary>
        public int From { get; }

        /// <summary>
        ///     The inclusive end id of the target range.
        /// </summary>
        public int To { get; }

        /// <summary>
        ///     The type of the range
        /// </summary>
        public CommandTargetType Type { get; set; }
    }
}
