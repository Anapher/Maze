using System;
using System.Windows;
using Tasks.Common.Shared.Commands;
using Tasks.Infrastructure.Administration.Library.Command;

namespace Tasks.Common.Administration.Commands
{
    public class ServerTestDescription : ICommandDescription
    {
        public string Name { get; set; } = "Server Test";
        public string Namespace { get; } = null;
        public string Summary { get; set; } = "Test";
        public UIElement Icon { get; set; }
        public Type DtoType { get; set; } = typeof(ServerTestCommandInfo);
    }
}