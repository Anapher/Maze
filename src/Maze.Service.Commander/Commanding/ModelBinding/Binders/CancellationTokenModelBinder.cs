using System;
using System.Threading;
using System.Threading.Tasks;
using Maze.Service.Commander.Commanding.ModelBinding.Abstract;

namespace Maze.Service.Commander.Commanding.ModelBinding.Binders
{
    /// <summary>
    ///     <see cref="IModelBinder" /> implementation to bind models of type <see cref="CancellationToken" />.
    /// </summary>
    public class CancellationTokenModelBinder : IModelBinder
    {
        /// <inheritdoc />
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null) throw new ArgumentNullException(nameof(bindingContext));

            // We need to force boxing now, so we can insert the same reference to the boxed CancellationToken
            // in both the ValidationState and ModelBindingResult.
            //
            // DO NOT simplify this code by removing the cast.
            var model = (object) bindingContext.MazeContext.RequestAborted;
            bindingContext.Result = ModelBindingResult.Success(model);

            return Task.CompletedTask;
        }
    }
}