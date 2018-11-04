using System;
using System.Windows;

namespace Tasks.Infrastructure.Administration.Library
{
    public interface ITaskServiceDescription
    {
        string Name { get; }
        string Summary { get; }
        UIElement Icon { get; }
        Type DtoType { get; }
    }
}