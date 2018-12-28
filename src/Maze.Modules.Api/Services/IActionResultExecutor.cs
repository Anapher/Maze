using System.Threading.Tasks;
using Maze.Modules.Api.Response;

namespace Maze.Modules.Api.Services
{
    /// <summary>
    ///     Defines an interface for a service which can execute a particular kind of <see cref="IActionResult" /> by
    ///     manipulating the <see cref="MazeResponse" />.
    /// </summary>
    /// <typeparam name="TResult">The type of <see cref="IActionResult" />.</typeparam>
    /// <remarks>
    ///     Implementions of <see cref="IActionResultExecutor{TResult}" /> are typically called by the
    ///     <see cref="IActionResult.ExecuteResultAsync(ActionContext)" /> method of the corresponding action result type.
    ///     Implementations should be registered as singleton services.
    /// </remarks>
    public interface IActionResultExecutor<in TResult> where TResult : IActionResult
    {
        /// <summary>
        ///     Asynchronously excecutes the action result, by modifying the <see cref="MazeResponse" />.
        /// </summary>
        /// <param name="context">The <see cref="ActionContext" /> associated with the current request."/></param>
        /// <param name="result">The action result to execute.</param>
        /// <returns>A <see cref="Task" /> which represents the asynchronous operation.</returns>
        Task ExecuteAsync(ActionContext context, TResult result);
    }
}