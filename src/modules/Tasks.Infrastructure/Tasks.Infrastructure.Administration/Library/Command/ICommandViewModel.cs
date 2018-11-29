using Tasks.Infrastructure.Core.Commands;

namespace Tasks.Infrastructure.Administration.Library.Command
{
    /// <summary>
    ///     The view model of a command (based on a <see cref="CommandInfo"/> data transfer object)
    /// </summary>
    /// <typeparam name="TCommandInfo">The data transfer object of the command</typeparam>
    public interface ICommandViewModel<TCommandInfo> : ITaskServiceViewModel<TCommandInfo> where TCommandInfo : CommandInfo
    {
    }
}