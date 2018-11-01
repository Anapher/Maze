using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Tasks.Infrastructure.Administration.Library;

namespace Tasks.Infrastructure.Administration.Utilities
{
    public static class TaskServiceViewModelUtils
    {
        public static ValidationResult Validate(object viewModel, TaskContext taskContext)
        {
            var method = viewModel.GetType().GetMethod("Validate", BindingFlags.Instance | BindingFlags.Public);
            return (ValidationResult) method.Invoke(viewModel, new object[] {taskContext});
        }
    }
}