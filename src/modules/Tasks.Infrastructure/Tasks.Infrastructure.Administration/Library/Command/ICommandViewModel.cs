using Tasks.Infrastructure.Core.Commands;

namespace Tasks.Infrastructure.Administration.Library.Command
{
    public interface ICommandViewModel<TCommandInfo> : ITaskServiceViewModel<TCommandInfo> where TCommandInfo : CommandInfo
    {
    }
}