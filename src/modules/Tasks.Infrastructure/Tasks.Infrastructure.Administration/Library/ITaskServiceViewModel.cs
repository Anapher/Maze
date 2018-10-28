using System.ComponentModel.DataAnnotations;

namespace Tasks.Infrastructure.Administration.Library
{
    public interface ITaskServiceViewModel<TDto>
    {
        void Initialize(TDto model);
        ValidationResult Validate(TaskContext context);
        TDto Build(TaskContext context);
    }
}