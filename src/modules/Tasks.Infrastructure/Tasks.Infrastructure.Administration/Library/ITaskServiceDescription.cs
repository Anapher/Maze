using System;
using System.Windows;

namespace Tasks.Infrastructure.Administration.Library
{
    public interface ITaskServiceDescription<in TDto>
    {
        string Name { get; }
        string Summary { get; }
        UIElement Icon { get; }
        Type DtoType { get; }

        string Describe(TDto dto);
    }
}