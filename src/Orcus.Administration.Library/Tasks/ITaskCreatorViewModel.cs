using System;
using System.ComponentModel.DataAnnotations;
using System.Windows;

namespace Orcus.Administration.Library.Tasks
{
    public interface ITaskDescriber
    {
        string Name { get; }
        string Summary { get; }
        UIElement Icon { get; }
        Type CommandInfoType { get; }
    }
    
    public interface ITaskCreatorViewModel<TCommandInfo>
    {
        void Initialize(TCommandInfo model);
        ValidationResult Validate(TaskContext context);
        TCommandInfo Build(TaskContext context);
    }

    public interface ITaskCreatorViewProvider
    {
        UIElement GetView(object taskCreatorViewModel);
    }
}