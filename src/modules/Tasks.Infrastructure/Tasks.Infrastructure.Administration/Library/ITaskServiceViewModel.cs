using System.ComponentModel.DataAnnotations;
using Tasks.Infrastructure.Core;

namespace Tasks.Infrastructure.Administration.Library
{
    public interface ITaskServiceViewModel<TDto>
    {
        /// <summary>
        ///     Initialize the view model using an existing <see cref="TDto"/> (to edit that object)
        /// </summary>
        /// <param name="model">The existing data transfer object that should be edited</param>
        void Initialize(TDto model);

        /// <summary>
        ///     Validate the input field of the <see cref="TDto"/>
        /// </summary>
        /// <returns>Return the validation result that carries possible errors</returns>
        ValidationResult ValidateInput();

        /// <summary>
        ///     Validate the context of the task that should be created. This method is executed after <see cref="ValidateInput"/> of all serviced succeeded.
        /// </summary>
        /// <param name="orcusTask">The complete orcus task that is about to be created.</param>
        /// <returns>Return the validation result that carries possible errors</returns>
        ValidationResult ValidateContext(OrcusTask orcusTask);

        /// <summary>
        ///     Build the data transfer object based on the information that were validated in <see cref="ValidateInput"/>
        /// </summary>
        /// <returns>Return a data transfer object that contains the required information for the service to execute.</returns>
        TDto Build();
    }
}