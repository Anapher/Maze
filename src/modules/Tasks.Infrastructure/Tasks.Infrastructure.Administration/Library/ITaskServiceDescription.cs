using System;
using System.Windows;
using Unclassified.TxLib;

namespace Tasks.Infrastructure.Administration.Library
{
    public interface ITaskServiceDescription
    {
        /// <summary>
        ///     The name
        /// </summary>
        string Name { get; }

        /// <summary>
        ///     The service namespace, separated by the '.' (dot). The segments will be translated by <see cref="Tx"/> using the key template "Tasks.Namespace.{segment}"
        /// </summary>
        string Namespace { get; }

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