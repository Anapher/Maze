using System;
using System.Linq.Expressions;
using Unclassified.TxLib;

namespace Tasks.Infrastructure.Administration.PropertyGrid
{
    /// <summary>
    ///     Makes it easy to initialize properties on a <see cref="IProvideEditableProperties" />
    /// </summary>
    public static class FluentCommandPropertyHelper
    {
        /// <summary>
        ///     Register a new property for the <see cref="IProvideEditableProperties" />
        /// </summary>
        /// <typeparam name="T">The type of the property</typeparam>
        /// <param name="provideEditableProperties">
        ///     The <see cref="IProvideEditableProperties" /> the property should be registered
        ///     to
        /// </param>
        /// <param name="property">The expression which points to the property</param>
        /// <param name="name">The display name of the property</param>
        /// <param name="description">The description of the property</param>
        /// <param name="category">The category of the property</param>
        /// <returns>Returns the <see cref="provideEditableProperties" /> to allow the fluent usage of this method</returns>
        public static IProvideEditableProperties RegisterProperty<T>(this IProvideEditableProperties provideEditableProperties,
            Expression<Func<T>> property, string name, string description, string category)
        {
            provideEditableProperties.Properties.Add(new Property<T>(provideEditableProperties, property, name, description, category ?? Tx.T("TasksInfrastructure:CreateTask.Common")));
            return provideEditableProperties;
        }

        /// <summary>
        ///     Register a new property for the <see cref="IProvideEditableProperties" /> by convention. The description text key will be <param name="name">name</param>.Description.
        /// </summary>
        /// <typeparam name="T">The type of the property</typeparam>
        /// <param name="provideEditableProperties">
        ///     The <see cref="IProvideEditableProperties" /> the property should be registered
        ///     to
        /// </param>
        /// <param name="property">The expression which points to the property</param>
        /// <param name="name">The tx key of the display name of the property</param>
        /// <param name="category">The category of the property</param>
        /// <returns>Returns the <see cref="provideEditableProperties" /> to allow the fluent usage of this method</returns>
        public static IProvideEditableProperties RegisterPropertyByConvention<T>(this IProvideEditableProperties provideEditableProperties,
            Expression<Func<T>> property, string name, string category = null)
        {
            provideEditableProperties.RegisterProperty(property, Tx.T(name), $"{name}.Description",
                category ?? Tx.T("TasksInfrastructure:CreateTask.Common"));
            return provideEditableProperties;
        }
    }
}