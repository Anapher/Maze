using System;
using System.Windows;
using Anapher.Wpf.Toolkit.Windows;
using TaskDialogInterop;
using Unclassified.TxLib;

namespace Maze.Administration.Library.Extensions
{
    /// <summary>
    ///     Provide extensions for <see cref="IWindowInteractionService"/>
    /// </summary>
    public static class WindowInteractionExtensions
    {
        /// <summary>
        ///     Show an error message with the <see cref="MessageBoxImage.Error"/> icon and the word "Error" as caption (correctly translated)
        /// </summary>
        /// <param name="window">The window the errors should be displayed on</param>
        /// <param name="message">The message text.</param>
        public static void ShowErrorMessageBox(this IWindowInteractionService window, string message)
        {
            window.ShowMessageBox(message, Tx.T("Error"), MessageBoxButton.OK, MessageBoxImage.Error,
                MessageBoxResult.None, MessageBoxOptions.None);
        }

        /// <summary>
        ///     Show the <see cref="Exception.Message"/> in a message box  with the <see cref="MessageBoxImage.Error"/> icon and the word "Error" as caption (correctly translated)
        /// </summary>
        /// <param name="window">The window the errors should be displayed on</param>
        /// <param name="e"></param>
        public static void ShowErrorMessageBox(this IWindowInteractionService window, Exception e)
        {
            window.ShowMessageBox(e.Message, Tx.T("Error"), MessageBoxButton.OK, MessageBoxImage.Error,
                MessageBoxResult.None, MessageBoxOptions.None);
        }

        /// <summary>
        ///     Show a <see cref="TaskDialog"/> on an <see cref="IWindowInteractionService"/>
        /// </summary>
        /// <param name="window">The window that owns the <see cref="TaskDialog"/>.</param>
        /// <param name="options">The options of the <see cref="TaskDialog"/></param>
        /// <returns>Return the result of the <see cref="TaskDialog"/></returns>
        public static TaskDialogResult ShowTaskDialog(this IWindowInteractionService window, TaskDialogOptions options)
        {
            options.Owner = (Window)window; //safe cast as Maze doesn't use other windows
            return TaskDialog.Show(options);
        }

        /// <summary>
        ///     Show an information message with the <see cref="MessageBoxImage.Information"/> icon and the word "Information" as caption (correctly translated)
        /// </summary>
        /// <param name="window">The window the message should be displayed on</param>
        /// <param name="message">The message text.</param>
        public static void ShowInformation(this IWindowInteractionService window, string message)
        {
            window.ShowMessageBox(message, Tx.T("Information"), MessageBoxButton.OK, MessageBoxImage.Information,
                MessageBoxResult.OK, MessageBoxOptions.None);
        }
    }
}