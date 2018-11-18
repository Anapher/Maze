using Tasks.Infrastructure.Core.Dtos;

namespace Tasks.Infrastructure.Administration.Library.Result
{
    public interface ICommandResultViewModel
    {
        void Initialize(CommandResultDto commandResult);
    }
}