using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Tasks.Infrastructure.Administration.Library;
using Tasks.Infrastructure.Core;

namespace Tasks.Infrastructure.Administration.Utilities
{
    public static class TaskServiceViewModelUtils
    {
        public static ValidationResult ValidateInput(object viewModel)
        {
            var method = viewModel.GetType().GetMethod(nameof(ITaskServiceViewModel<string>.ValidateInput), BindingFlags.Instance | BindingFlags.Public);
            return (ValidationResult) method.Invoke(viewModel, new object[0]);
        }

        public static ValidationResult ValidateContext(object viewModel, OrcusTask orcusTask)
        {
            var method = viewModel.GetType().GetMethod(nameof(ITaskServiceViewModel<string>.ValidateContext), BindingFlags.Instance | BindingFlags.Public);
            return (ValidationResult) method.Invoke(viewModel, new object[] {orcusTask});
        }

        public static void Initialize(object viewModel, object dto)
        {
            var method = viewModel.GetType().GetMethod(nameof(ITaskServiceViewModel<string>.Initialize), BindingFlags.Instance | BindingFlags.Public);
            method.Invoke(viewModel, new[] { dto });
        }

        public static T Build<T>(object viewModel, TaskContext taskContext)
        {
            var method = viewModel.GetType().GetMethod(nameof(ITaskServiceViewModel<string>.Build), BindingFlags.Instance | BindingFlags.Public);
            return (T) method.Invoke(viewModel, new object[] {taskContext});
        }
    }
}