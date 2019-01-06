using System;
using System.Net.Http;
using System.Windows;
using Tasks.Infrastructure.Administration.Library.Result;
using Tasks.Infrastructure.Administration.Views.TaskOverview.ResultView;
using Tasks.Infrastructure.Core.Dtos;

namespace Tasks.Infrastructure.Administration.ViewModels.TaskOverview.ResultView
{
    public class ExceptionResultViewProvider : ICommandResultViewProvider
    {
        public int Priority { get; set; } = 100000;

        public UIElement GetView(HttpResponseMessage responseMessage, CommandResultDto dto, IServiceProvider serviceProvider)
        {
            if (dto.Status != null)
                return null;

            return new ExceptionView {DataContext = new ExceptionViewModel(dto)};
        }
    }
}