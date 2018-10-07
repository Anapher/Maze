using System.ComponentModel.DataAnnotations;
using Orcus.Server.Connection.Tasks.Commands;
using Prism.Mvvm;

namespace Orcus.Administration.Library.Tasks
{
    public abstract class TaskCreatorViewModel<TCommandInfo> : BindableBase, ITaskCreatorViewModel where TCommandInfo : CommandInfo
    {
        protected abstract void Initialize(TCommandInfo model);
        public abstract ValidationResult Validate(TaskContext context);
        protected abstract TCommandInfo InternalBuild(TaskContext context);

        public virtual void Initialize(CommandInfo model)
        {
            Initialize((TCommandInfo) model);
        }

        public CommandInfo Build(TaskContext context) => InternalBuild(context);
    }
}