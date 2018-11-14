using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using Anapher.Wpf.Swan.Extensions;
using Anapher.Wpf.Swan.ViewInterface;
using Orcus.Administration.Library.Exceptions;
using Orcus.Server.Connection.Error;
using TaskDialogInterop;
using Unclassified.TxLib;

namespace Orcus.Administration.Library.Extensions
{
    public static class TaskErrorExtensions
    {
        public static async Task<bool> EnumerateItems<T>(this IList<T> items, IWindowInteractionService window, Func<T, Task> action)
        {
            var dontShowErrors = false;
            for (var i = 0; i < items.Count; i++)
            {
                var licenseViewModel = items[i];

                try
                {
                    await action(licenseViewModel);
                }
                catch (Exception e)
                {
                    if (!e.ShowErrorMessageContinue(window, ref dontShowErrors, i == items.Count - 1))
                        return false;
                }
            }

            return true;
        }

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

        public static void ShowMessage(this Exception exception, IWindowInteractionService window)
        {
            if (exception is RestException restException)
                restException.ShowMessage(window);
            else
                window.ShowErrorMessageBox(Tx.T("MainWindow:UnexpectedErrorOccurred", "message", exception.Message));
        }

        public static void ShowMessage(this RestException exception, IWindowInteractionService window)
        {
            window.ShowErrorMessageBox(GetRestExceptionMessage(exception));
        }

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