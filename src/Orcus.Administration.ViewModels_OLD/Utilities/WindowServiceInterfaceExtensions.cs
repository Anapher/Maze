using System;
using System.Windows;
using Anapher.Wpf.Swan.ViewInterface;
using TaskDialogInterop;
using Unclassified.TxLib;

namespace Orcus.Administration.ViewModels.Utilities
{
    public static class WindowServiceInterfaceExtensions
    {
        public static void ShowErrorMessage(this IWindowServiceInterface windowServiceInterface, string message)
        {
            windowServiceInterface.ShowMessageBox(message, Tx.T("Error"), MessageBoxButton.OK,
                MessageBoxImage.Error);
        }

        public static void ShowErrorMessage(this IWindowServiceInterface windowServiceInterface, Exception e)
        {
            windowServiceInterface.ShowMessageBox(e.Message, Tx.T("Error"), MessageBoxButton.OK,
                MessageBoxImage.Error);
        }

        public static TaskDialogResult ShowTaskDialog(this IWindowServiceInterface windowServiceInterface,
            TaskDialogOptions options)
        {
            TaskDialogResult result = null;
            windowServiceInterface.ShowDialog(x =>
            {
                options.Owner = x;
                result = TaskDialog.Show(options);
                return true;
            });
            return result;
        }
    }
}