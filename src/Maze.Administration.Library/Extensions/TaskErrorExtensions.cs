using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using Anapher.Wpf.Swan.Extensions;
using Anapher.Wpf.Swan.ViewInterface;
using Maze.Administration.Library.Exceptions;
using Maze.Server.Connection.Error;
using TaskDialogInterop;
using Unclassified.TxLib;

namespace Maze.Administration.Library.Extensions
{
    /// <summary>
    ///     Extensions for <see cref="Task"/> that help to catch and report errors
    /// </summary>
    public static class TaskErrorExtensions
    {
        /// <summary>
        ///     Enumerate a <see cref="IEnumerable{T}"/> and execute an asynchronous <see cref="action"/> on them. Display errors as a message box to the user with the option to cancel the process or to hide the errors and just conintue always.
        /// </summary>
        /// <typeparam name="T">The type of the item</typeparam>
        /// <param name="items">The items that should be processed</param>
        /// <param name="window">The window the errors should be displayed on</param>
        /// <param name="action">The action that should be executed on every item.</param>
        /// <returns>Return false if the user cancelled the process.</returns>
        public static async Task<bool> EnumerateItems<T>(this IList<T> items, IWindowInteractionService window, Func<T, Task> action)
        {
            var dontShowErrors = false;
            for (var i = 0; i < items.Count; i++)
            {
                var item = items[i];

                try
                {
                    await action(item);
                }
                catch (Exception e)
                {
                    if (!e.ShowErrorMessageContinue(window, ref dontShowErrors, i == items.Count - 1))
                        return false;
                }
            }

            return true;
        }

        /// <summary>
        ///     Await a task, catch and show any resulting errors to the user. Enable the possibility to suppress the display of the error next time.
        /// </summary>
        /// <param name="task">The task that should be executed</param>
        /// <param name="window">The window the errors should be displayed on</param>
        /// <param name="dontShowAgain">The <see cref="DoNotAskAgainInfo"/> object that keeps the status whether the error should be shown to the user or just swallowed.</param>
        /// <param name="isLastItem">True if the current item is the last item. If this value is true, the option to suppress further displays of errors will be disabled.</param>
        /// <returns>Return whether an error occurred (<code>false</code>)</returns>
        public static async Task<bool> OnErrorShowMessageBox(this Task task, IWindowInteractionService window,
            DoNotAskAgainInfo dontShowAgain, bool isLastItem = false)
        {
            try
            {
                await task;
                return true;
            }
            catch (Exception e)
            {
                var dontShowAgainVal = dontShowAgain.DoNotAskAgain;
                var result = e.ShowErrorMessageContinue(window, ref dontShowAgainVal, isLastItem);
                dontShowAgain.DoNotAskAgain = dontShowAgainVal;
                return result;
            }
        }

        /// <summary>
        /// Await a task, catch and show any resulting errors to the user.
        /// </summary>
        /// <param name="task">The task that should be executed</param>
        /// <param name="window">The window the errors should be displayed on</param>
        /// <returns>Return whether an error occurred (<code>false</code>)</returns>
        public static async Task<bool> OnErrorShowMessageBox(this Task task, IWindowInteractionService window)
        {
            try
            {
                await task;
                return true;
            }
            catch (Exception e)
            {
                e.ShowMessage(window);
                return false;
            }
        }

        /// <summary>
        /// Await a task, catch and show any resulting errors to the user.
        /// </summary>
        /// <param name="task">The task that should be executed</param>
        /// <param name="window">The window the errors should be displayed on</param>
        public static async void OnErrorShowMessageBoxAsync(this Task task, IWindowInteractionService window)
        {
            try
            {
                await task;
            }
            catch (Exception e)
            {
                e.ShowMessage(window);
            }
        }

        /// <summary>
        /// Await a task, catch and show any resulting errors to the user and return the result.
        /// </summary>
        /// <typeparam name="T">The type of the result value of the task.</typeparam>
        /// <param name="task">The task that should be executed.</param>
        /// <param name="window">The window the errors should be displayed on</param>
        /// <returns><see cref="task"/>Return the result of the </returns>
        public static async Task<SuccessOrError<T>> OnErrorShowMessageBox<T>(this Task<T> task, IWindowInteractionService window)
        {
            T result;
            try
            {
                result = await task;
            }
            catch (RestException e)
            {
                e.ShowMessage(window);
                return SuccessOrError<T>.DefaultFailed;
            }
            catch (Exception e)
            {
                e.ShowMessage(window);
                return SuccessOrError<T>.DefaultFailed;
            }

            return new SuccessOrError<T>(result);
        }

        /// <summary>
        ///     Show an error message that displays the <see cref="exception"/> and asks the user whether he wants to continue.
        /// </summary>
        /// <param name="exception">The exception that occurred.</param>
        /// <param name="window">The window the error should be displayed on</param>
        /// <returns>Return true if the user selected that he wants to continue.</returns>
        public static bool ShowErrorMessageContinue(this Exception exception, IWindowInteractionService window)
        {
            if (exception is RestException restException)
                return window.ShowMessage(
                           Tx.T("MainWindow:RestErrorOccurredContinue", "message",
                               GetRestExceptionMessage(restException)), Tx.T("Error"), MessageBoxButton.OKCancel,
                           MessageBoxImage.Error) == MessageBoxResult.OK;

            return window.ShowMessage(Tx.T("MainWindow:UnexpectedErrorOccurredContinue", "message", exception.Message),
                       Tx.T("Error"), MessageBoxButton.OKCancel, MessageBoxImage.Error) == MessageBoxResult.OK;
        }

        /// <summary>
        ///     Show an error message to the user and asks if he wants to continue. Also, the user will be asked if he wants to decide the next time or just always continue.
        /// </summary>
        /// <param name="exception">The exception that just occurred.</param>
        /// <param name="window">The window the error should be displayed on</param>
        /// <param name="dontShowAgain">A link to a <see cref="bool"/> that indicates if the user wants to see the next exception. This field will be read (and if true, no action will be taken) and changed.</param>
        /// <param name="isLastItem">True if the current item is the last item. If true, the user won't be asked if he wants to suppress the next exception as this is the last exception possible.</param>
        /// <returns>Return if the user wants to continue. Return false if the user wants to cancel the process.</returns>
        public static bool ShowErrorMessageContinue(this Exception exception, IWindowInteractionService window, ref bool dontShowAgain,
            bool isLastItem = false)
        {
            if (!dontShowAgain)
                return true;

            string message;
            if (exception is RestException restException)
                message = Tx.T("MainWindow:RestErrorOccurredContinue", "message",
                    GetRestExceptionMessage(restException));
            else
                message = Tx.T("MainWindow:UnexpectedErrorOccurredContinue", "message", exception.Message);

            var options = new TaskDialogOptions
            {
                AllowDialogCancellation = false,
                Title = Tx.T("Error"),
                Content = message,
                CommonButtons = TaskDialogCommonButtons.OKCancel,
                MainIcon = VistaTaskDialogIcon.Error
            };

            if (!isLastItem)
                options.VerificationText = Tx.T("DoNotAskMeAgainForCurrentOperation");

            var result = window.ShowTaskDialog(options);
            if (result.Result == TaskDialogSimpleResult.Ok)
            {
                dontShowAgain = result.VerificationChecked == true;
                return true;
            }

            return false;
        }

        /// <summary>
        ///  Show a messagebox that displays the <see cref="Exception"/>. <see cref="RestException"/>s will be propaply handled.
        /// </summary>
        /// <param name="exception">The exception that should be displayed.</param>
        /// <param name="window">The window the error should be displayed on</param>
        public static void ShowMessage(this Exception exception, IWindowInteractionService window)
        {
            if (exception is RestException restException)
                restException.ShowMessage(window);
            else
                window.ShowErrorMessageBox(Tx.T("MainWindow:UnexpectedErrorOccurred", "message", exception.Message));
        }

        /// <summary>
        ///     Show a message box that displays the <see cref="RestException"/>
        /// </summary>
        /// <param name="exception">The exception that should be displayed</param>
        /// <param name="window">The window the error should be displayed on</param>
        public static void ShowMessage(this RestException exception, IWindowInteractionService window)
        {
            window.ShowErrorMessageBox(GetRestExceptionMessage(exception));
        }

        /// <summary>
        ///     Attempt to find a translation for a <see cref="RestException"/> or return the english message
        /// </summary>
        /// <param name="exception">The rest exception</param>
        /// <returns>Return the translated message if found, or the default message.</returns>
        public static string GetRestExceptionMessage(this RestException exception)
        {
            var keyName = "RestErrors:" + (ErrorCode) exception.ErrorId;
            if (!Tx.TryGetText(keyName, out var result))
            {
                return exception.Message;
            }

            return result;
        }
    }
}