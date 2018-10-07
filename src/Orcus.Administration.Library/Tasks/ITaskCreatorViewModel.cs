using System.ComponentModel.DataAnnotations;
using Orcus.Server.Connection.Tasks.Commands;

namespace Orcus.Administration.Library.Tasks
{
    public interface ITaskCreatorViewModel
    {
        void Initialize(CommandInfo model);

        ValidationResult Validate(TaskContext context);
        CommandInfo Build(TaskContext context);
    }
}