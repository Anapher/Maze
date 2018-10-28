using Tasks.Infrastructure.Core.Commands;

namespace Tasks.Infrastructure.Administration.Library
{
    public abstract class PropertyGridCommandCreatorViewModel<TCommand> : CommandCreatorViewModel<TCommand> where TCommand : CommandInfo
    {
    }
}