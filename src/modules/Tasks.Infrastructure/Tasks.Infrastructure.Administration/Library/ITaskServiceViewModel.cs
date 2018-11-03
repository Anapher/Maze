using System.ComponentModel.DataAnnotations;
using Tasks.Infrastructure.Core;

namespace Tasks.Infrastructure.Administration.Library
{
    public interface ITaskServiceViewModel<TDto>
    {
        void Initialize(TDto model);
        ValidationResult ValidateInput();
        ValidationResult ValidateContext(OrcusTask orcusTask);
        TDto Build();
    }
}