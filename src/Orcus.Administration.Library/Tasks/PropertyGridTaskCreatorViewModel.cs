using Orcus.Server.Connection.Tasks.Commands;

namespace Orcus.Administration.Library.Tasks
{
    public abstract class PropertyGridTaskCreatorViewModel<TCommand> : TaskCreatorViewModel<TCommand> where TCommand : CommandInfo
    {
    }
}