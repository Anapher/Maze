using System.Threading.Tasks;
using AutoMapper;
using CodeElements.BizRunner;
using CodeElements.BizRunner.Generic;
using Tasks.Infrastructure.Core.Dtos;
using Tasks.Infrastructure.Server.BusinessDataAccess;
using Tasks.Infrastructure.Management.Data;

namespace Tasks.Infrastructure.Server.Business
{
    public interface ICreateTaskCommandResultAction : IGenericActionAsync<CommandResultDto, CommandResult>
    {
    }

    public class CreateTaskCommandResultAction : BusinessActionErrors, ICreateTaskCommandResultAction
    {
        private readonly ICommandResultsDbAccess _dbAccess;

        public CreateTaskCommandResultAction(ICommandResultsDbAccess dbAccess)
        {
            _dbAccess = dbAccess;
        }

        public async Task<CommandResult> BizActionAsync(CommandResultDto inputData)
        {
            if (ValidateModelFailed(inputData))
                return default;

            var commandResult = Mapper.Map<CommandResult>(inputData);
            await _dbAccess.CreateAsync(commandResult);

            return commandResult;
        }
    }
}