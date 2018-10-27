using Tasks.Infrastructure.Core.Commands;

namespace Tasks.Infrastructure.Administration.Library
{
    public abstract class PropertyGridTaskCreatorViewModel<TCommand> : TaskCreatorViewModel<TCommand> where TCommand : CommandInfo
    {
    }
}