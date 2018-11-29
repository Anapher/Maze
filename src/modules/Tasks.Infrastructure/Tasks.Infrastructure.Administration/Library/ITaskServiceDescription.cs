using System;
using System.Windows;

namespace Tasks.Infrastructure.Administration.Library
{
    public interface ITaskServiceDescription
    {
        /// <summary>
        ///     The name
        /// </summary>
        string Name { get; }

        /// <summary>
        ///     A short summary about the functionality
        /// </summary>
        string Summary { get; }

        /// <summary>
        ///     An icon that represents the service
        /// </summary>
        UIElement Icon { get; }

        /// <summary>
        ///     The type of the data transfer object
        /// </summary>
        Type DtoType { get; }
    }
}