using Tasks.Infrastructure.Core.Dtos;

namespace Tasks.Infrastructure.Administration.ViewModels.TaskOverview.ResultView
{
    public class ExceptionViewModel
    {
        public ExceptionViewModel(CommandResultDto commandResult)
        {
            ExceptionMessage = commandResult.Result;
        }

        public string ExceptionMessage { get; }
    }
}