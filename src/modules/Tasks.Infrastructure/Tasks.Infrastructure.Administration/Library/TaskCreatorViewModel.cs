using System.ComponentModel.DataAnnotations;
using Prism.Mvvm;
using Tasks.Infrastructure.Core.Commands;

namespace Tasks.Infrastructure.Administration.Library
{
    public abstract class TaskCreatorViewModel<TCommandInfo> : BindableBase, ITaskCreatorViewModel<TCommandInfo> where TCommandInfo : CommandInfo
    {
        public abstract void Initialize(TCommandInfo model);
        public abstract ValidationResult Validate(TaskContext context);
        protected abstract TCommandInfo InternalBuild(TaskContext context);

        public virtual void Initialize(CommandInfo model)
        {
            Initialize((TCommandInfo) model);
        }

        public TCommandInfo Build(TaskContext context) => InternalBuild(context);
    }
}