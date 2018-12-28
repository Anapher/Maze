using System;
using Maze.Modules.Api;
using Maze.Modules.Api.ModelBinding;

// ReSharper disable once CheckNamespace
namespace Maze.Service.Commander.Commanding.ModelBinding.Abstract
{
    /// <summary>
    ///     A context that contains operating information for model binding and validation.
    /// </summary>
    public abstract class ModelBindingContext
    {
        /// <summary>
        ///     Represents the <see cref="Maze.Modules.Api.ActionContext" /> associated with this context.
        /// </summary>
        /// <remarks>
        ///     The property setter is provided for unit testing purposes only.
        /// </remarks>
        public abstract ActionContext ActionContext { get; set; }

        /// <summary>
        ///     Gets the <see cref="MazeContext" /> associated with this context.
        /// </summary>
        public MazeContext MazeContext => ActionContext?.Context;

        /// <summary>
        ///     Gets or sets the model value for the current operation.
        /// </summary>
        /// <remarks>
        ///     The <see cref="Model" /> will typically be set for a binding operation that works
        ///     against a pre-existing model object to update certain properties.
        /// </remarks>
        public abstract object Model { get; set; }

        /// <summary>
        ///     Gets the type of the model.
        /// </summary>
        /// <remarks>
        ///     The <see cref="ModelMetadata" /> property must be set to access this property.
        /// </remarks>
        public virtual Type ModelType => ModelMetadata.ModelType;

        /// <summary>
        ///     Gets or sets the <see cref="IValueProvider" /> associated with this context.
        /// </summary>
        public abstract IValueProvider ValueProvider { get; set; }

        /// <summary>
        ///     Gets or sets the name of the model. This property is used as a key for looking up values in
        ///     <see cref="IValueProvider" /> during model binding.
        /// </summary>
        public abstract string ModelName { get; set; }

        /// <summary>
        ///     <para>
        ///         Gets or sets a <see cref="ModelBindingResult" /> which represents the result of the model binding process.
        ///     </para>
        ///     <para>
        ///         Before an <see cref="IModelBinder" /> is called, <see cref="Result" /> will be set to a value indicating
        ///         failure. The binder should set <see cref="Result" /> to a value created with
        ///         <see cref="ModelBindingResult.Success" /> if model binding succeeded.
        ///     </para>
        /// </summary>
        public abstract ModelBindingResult Result { get; set; }

        /// <summary>
        ///     Gets or sets the metadata for the model associated with this context.
        /// </summary>
        public abstract ModelMetadata ModelMetadata { get; set; }

        /// <summary>
        ///     Represents the current error state
        /// </summary>
        public abstract ModelStateDictionary ModelState { get; set; }
    }
}