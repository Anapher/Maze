using System;
using System.Windows;
using Anapher.Wpf.Swan.ViewInterface;
using TaskDialogInterop;
using Unclassified.TxLib;

namespace Orcus.Administration.Library.Extensions
{
    public static class WindowExtensions
    {
        public static void ShowErrorMessageBox(this IWindowInteractionService window, string message)
        {
            window.ShowMessageBox(message, Tx.T("Error"), MessageBoxButton.OK, MessageBoxImage.Error,
                MessageBoxResult.None, MessageBoxOptions.None);
        }

        public static void ShowErrorMessageBox(this IWindowInteractionService window, Exception e)
        {
            window.ShowMessageBox(e.Message, Tx.T("Error"), MessageBoxButton.OK, MessageBoxImage.Error,
                MessageBoxResult.None, MessageBoxOptions.None);
        }

        public static TaskDialogResult ShowTaskDialog(this IWindowInteractionService window, TaskDialogOptions options)
        {
            options.Owner = (Window)window; //safe cast as Orcus doesn't use other windows
            return TaskDialog.Show(options);
        }

        public static void ShowInformation(this IWindowInteractionService window, string message)
        {
            window.ShowMessageBox(message, Tx.T("Information"), MessageBoxButton.OK, MessageBoxImage.Information,
                MessageBoxResult.OK, MessageBoxOptions.None);
        }
    }
}