using System;
using System.Net.Http;
using System.Windows;
using Tasks.Infrastructure.Core.Dtos;

namespace Tasks.Infrastructure.Administration.Library.Result
{
    public interface ICommandResultViewProvider
    {
        int Priority { get; }
        UIElement GetView(HttpResponseMessage responseMessage, CommandResultDto dto, IServiceProvider serviceProvider);
    }
}