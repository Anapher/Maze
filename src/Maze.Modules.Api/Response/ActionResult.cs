using System.Threading.Tasks;

namespace Maze.Modules.Api.Response
{
    /// <summary>
    ///     A default implementation of <see cref="T:Microsoft.AspNetCore.Mvc.IActionResult" />.
    /// </summary>
    public abstract class ActionResult : IActionResult
    {
        /// <summary>
        ///     Executes the result operation of the action method asynchronously. This method is called by MVC to process
        ///     the result of an action method.
        ///     The default implementation of this method calls the
        ///     <see cref="M:Microsoft.AspNetCore.Mvc.ActionResult.ExecuteResult(Microsoft.AspNetCore.Mvc.ActionContext)" /> method
        ///     and
        ///     returns a completed task.
        /// </summary>
        /// <param name="context">
        ///     The context in which the result is executed. The context information includes
        ///     information about the action that was executed and request information.
        /// </param>
        /// <returns>A task that represents the asynchronous execute operation.</returns>
        public virtual Task ExecuteResultAsync(ActionContext context)
        {
            ExecuteResult(context);
            return Task.CompletedTask;
        }

        /// <summary>
        ///     Executes the result operation of the action method synchronously. This method is called by MVC to process
        ///     the result of an action method.
        /// </summary>
        /// <param name="context">
        ///     The context in which the result is executed. The context information includes
        ///     information about the action that was executed and request information.
        /// </param>
        public virtual void ExecuteResult(ActionContext context)
        {
        }
    }
}