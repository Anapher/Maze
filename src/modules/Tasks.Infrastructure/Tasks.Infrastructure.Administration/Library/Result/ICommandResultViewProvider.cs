using System.Net.Http;
using System.Windows;
using Autofac;
using Tasks.Infrastructure.Core.Dtos;

namespace Tasks.Infrastructure.Administration.Library.Result
{
    public interface ICommandResultViewProvider
    {
        int Priority { get; }
        UIElement GetView(HttpResponseMessage responseMessage, CommandResultDto dto, IComponentContext context);
    }
}